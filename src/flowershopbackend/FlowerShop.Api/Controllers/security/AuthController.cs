using FlowerShop.Application.Dtos.Auth;
using FlowerShop.Application.Features.Auth.Commands;
using Microsoft.AspNetCore.Mvc;

namespace FlowerShop.Api.Controllers;

/// <summary>
/// Handles user authentication: registration, login, and email verification.
/// All endpoints are publicly accessible (no [Authorize] required).
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ILogger<AuthController> _logger;
    private readonly IRegisterUserCommand<RegisterDto, AuthResponseDto> _registerCommand;
    private readonly ILoginCommand<LoginDto, AuthResponseDto?> _loginCommand;
    private readonly IConfirmEmailCommand<ConfirmEmailDto, bool> _confirmEmailCommand;

    public AuthController(
        ILogger<AuthController> logger,
        IRegisterUserCommand<RegisterDto, AuthResponseDto> registerCommand,
        ILoginCommand<LoginDto, AuthResponseDto?> loginCommand,
        IConfirmEmailCommand<ConfirmEmailDto, bool> confirmEmailCommand)
    {
        _logger = logger;
        _registerCommand = registerCommand;
        _loginCommand = loginCommand;
        _confirmEmailCommand = confirmEmailCommand;
    }

    /// <summary>
    /// Registers a new user account with the Customer role and sends a verification email.
    /// </summary>
    /// <param name="request">Registration details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A JWT and basic profile info on success.</returns>
    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register(
        [FromBody] RegisterDto request,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            _logger.LogInformation("Registering new user with email {Email}", request.Email);
            var result = await _registerCommand.Handle(request, cancellationToken);
            return StatusCode(StatusCodes.Status201Created, result);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning("Registration failed for {Email}: {Message}", request.Email, ex.Message);
            return Conflict(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Authenticates an existing user and returns a JWT with their role claim.
    /// </summary>
    /// <param name="request">Login credentials.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A JWT and basic profile info, or 401 if credentials are invalid.</returns>
    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Login(
        [FromBody] LoginDto request,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        _logger.LogInformation("Login attempt for {Email}", request.Email);
        var result = await _loginCommand.Handle(request, cancellationToken);
        if (result is null)
        {
            return Unauthorized(new { message = "Invalid email or password." });
        }

        return Ok(result);
    }

    /// <summary>
    /// Confirms a user's email address using the token from the verification link.
    /// </summary>
    /// <param name="request">DTO containing the raw token.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>200 OK on success, or 400 if the token is invalid or expired.</returns>
    [HttpPost("confirm-email")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ConfirmEmail(
        [FromBody] ConfirmEmailDto request,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        _logger.LogInformation("Email confirmation attempt");
        var success = await _confirmEmailCommand.Handle(request, cancellationToken);
        if (!success)
        {
            return BadRequest(new { message = "The verification token is invalid or has expired." });
        }

        return Ok(new { message = "Email verified successfully. Your account is now active." });
    }
}
