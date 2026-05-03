using System.Security.Cryptography;
using System.Text;
using FlowerShop.Application.Common;
using FlowerShop.Application.Dtos.Auth;
using FlowerShop.Application.Interfaces;
using FlowerShop.Domain.Entities;
using FlowerShop.Domain.Interfaces;

namespace FlowerShop.Application.Features.Auth.Commands;

// ---------------------------------------------------------------------------
// Register
// ---------------------------------------------------------------------------

/// <summary>
/// Command contract for registering a new user account.
/// </summary>
public interface IRegisterUserCommand<TIn, TOut>
{
    Task<TOut> Handle(TIn request, CancellationToken cancellationToken);
}

/// <summary>
/// Creates a new user with the <c>Customer</c> role, stores a salted SHA-256 password hash,
/// issues an email verification token and returns a JWT so the caller can immediately make
/// authenticated requests.
/// </summary>
public class RegisterUserCommand : IRegisterUserCommand<RegisterDto, AuthResponseDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJwtService _jwtService;
    private readonly IEmailService _emailService;

    public RegisterUserCommand(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        IJwtService jwtService,
        IEmailService emailService)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _jwtService = jwtService;
        _emailService = emailService;
    }

    /// <summary>
    /// Handles the register request.
    /// </summary>
    /// <param name="request">Registration details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>An <see cref="AuthResponseDto"/> containing a JWT for the new user.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the email is already in use.</exception>
    public async Task<AuthResponseDto> Handle(RegisterDto request, CancellationToken cancellationToken)
    {
        // Ensure email uniqueness
        var existing = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (existing is not null)
        {
            throw new InvalidOperationException($"Email '{request.Email}' is already registered.");
        }

        // Hash password: salt:hash (both Base64)
        var passwordHash = HashPassword(request.Password);

        var user = new User
        {
            Name = request.Name,
            Phone = request.Phone,
            Email = request.Email,
            PasswordHash = passwordHash,
            DeliveryAddress = request.DeliveryAddress,
            Status = UserStatus.Pending,
            EmailVerified = true, // Deactivate email verification for now
            CreationDate = DateTime.UtcNow,
            Role = "Customer"
        };

        var created = await _userRepository.AddAsync(user, cancellationToken);

        // Generate raw token (sent to user) and store its hash
        var rawToken = GenerateRawToken();
        var tokenHash = TokenHasher.Hash(rawToken);

        // var verificationToken = new EmailVerificationToken
        // {
        //     UserId = created.Id,
        //     TokenHash = tokenHash,
        //     ExpiresAt = DateTime.UtcNow.AddHours(24),
        //     IsUsed = false,
        //     CreationDate = DateTime.UtcNow
        // };

        // await _userRepository.AddVerificationTokenAsync(verificationToken, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Deactivate this feature for now
        // Send verification email (fire-and-forget style; errors are logged inside the service)
        // await _emailService.SendVerificationEmailAsync(created.Email, created.Name, rawToken, cancellationToken);

        var jwt = _jwtService.GenerateToken(created.Id, created.Email, created.Name, created.Role);
        return new AuthResponseDto(jwt, created.Email, created.Name);
    }

    // ------------------------------------------------------------------
    // Private helpers
    // ------------------------------------------------------------------

    private static string HashPassword(string password)
    {
        var saltBytes = RandomNumberGenerator.GetBytes(32);
        var salt = Convert.ToBase64String(saltBytes);
        var hash = Convert.ToBase64String(
            SHA256.HashData(Encoding.UTF8.GetBytes(password + salt)));
        return $"{salt}:{hash}";
    }

    private static string GenerateRawToken()
        => Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
}

// ---------------------------------------------------------------------------
// Login
// ---------------------------------------------------------------------------

/// <summary>
/// Command contract for authenticating an existing user.
/// </summary>
public interface ILoginCommand<TIn, TOut>
{
    Task<TOut> Handle(TIn request, CancellationToken cancellationToken);
}

/// <summary>
/// Validates the user's credentials and returns a JWT on success.
/// </summary>
public class LoginCommand : ILoginCommand<LoginDto, AuthResponseDto?>
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtService _jwtService;

    public LoginCommand(IUserRepository userRepository, IJwtService jwtService)
    {
        _userRepository = userRepository;
        _jwtService = jwtService;
    }

    /// <summary>
    /// Handles the login request.
    /// </summary>
    /// <param name="request">Login credentials.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>
    /// An <see cref="AuthResponseDto"/> when credentials are valid; <c>null</c> otherwise.
    /// </returns>
    public async Task<AuthResponseDto?> Handle(LoginDto request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (user is null)
        {
            return null;
        }

        if (!VerifyPassword(request.Password, user.PasswordHash))
        {
            return null;
        }

        var jwt = _jwtService.GenerateToken(user.Id, user.Email, user.Name, user.Role);
        return new AuthResponseDto(jwt, user.Email, user.Name);
    }

    private static bool VerifyPassword(string plainPassword, string storedHash)
    {
        var parts = storedHash.Split(':');
        if (parts.Length != 2)
        {
            return false;
        }

        var salt = parts[0];
        var expectedHash = parts[1];
        var actualHash = Convert.ToBase64String(
            SHA256.HashData(Encoding.UTF8.GetBytes(plainPassword + salt)));

        return actualHash == expectedHash;
    }
}

// ---------------------------------------------------------------------------
// Confirm Email
// ---------------------------------------------------------------------------

/// <summary>
/// Command contract for confirming a user's email address.
/// </summary>
public interface IConfirmEmailCommand<TIn, TOut>
{
    Task<TOut> Handle(TIn request, CancellationToken cancellationToken);
}

/// <summary>
/// Validates the one-time verification token, marks the user's email as verified
/// and activates their account.
/// </summary>
public class ConfirmEmailCommand : IConfirmEmailCommand<ConfirmEmailDto, bool>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ConfirmEmailCommand(IUserRepository userRepository, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// Handles email confirmation.
    /// </summary>
    /// <param name="request">DTO containing the raw token from the verification link.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns><c>true</c> if the token was valid and the account activated; <c>false</c> otherwise.</returns>
    public async Task<bool> Handle(ConfirmEmailDto request, CancellationToken cancellationToken)
    {
        var tokenHash = TokenHasher.Hash(request.Token);

        var verificationToken = await _userRepository.GetVerificationTokenAsync(tokenHash, cancellationToken);
        if (verificationToken is null)
        {
            return false;
        }

        if (verificationToken.IsUsed || verificationToken.ExpiresAt < DateTime.UtcNow)
        {
            return false;
        }

        verificationToken.IsUsed = true;
        var user = verificationToken.User;
        user.EmailVerified = true;
        user.Status = UserStatus.Active;

        await _userRepository.UpdateAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
