using System.Net;
using System.Net.Mail;
using FlowerShop.Application.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FlowerShop.Infrastructure.Auth;

/// <summary>
/// Sends transactional emails via SMTP.
/// When SMTP is not configured the token is logged to the console so that
/// local development does not require a mail server.
/// </summary>
public class SmtpEmailService : IEmailService
{
    private readonly SmtpOptions _options;
    private readonly ILogger<SmtpEmailService> _logger;

    public SmtpEmailService(IOptions<SmtpOptions> options, ILogger<SmtpEmailService> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    /// <summary>
    /// Sends a verification email containing a confirmation link.
    /// Falls back to console logging when SMTP host is not configured.
    /// </summary>
    /// <param name="toEmail">Recipient email address.</param>
    /// <param name="toName">Recipient display name.</param>
    /// <param name="token">The raw (un-hashed) verification token to embed in the link.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task SendVerificationEmailAsync(
        string toEmail,
        string toName,
        string token,
        CancellationToken cancellationToken = default)
    {
        var confirmationLink = $"{_options.FrontendBaseUrl}/confirm-email?token={Uri.EscapeDataString(token)}";

        if (string.IsNullOrWhiteSpace(_options.Host))
        {
            // Dev-mode: log instead of sending
            _logger.LogInformation(
                "[DEV] Email verification token for {Email}: {Link}",
                toEmail,
                confirmationLink);
            return;
        }

        var body =
            $"Hello {toName},\n\n" +
            $"Thank you for registering with Flower Shop.\n\n" +
            $"Please verify your email address by clicking the link below:\n\n" +
            $"{confirmationLink}\n\n" +
            $"This link expires in 24 hours.\n\n" +
            $"If you did not create an account, please ignore this email.\n\n" +
            $"Best regards,\n{_options.FromName}";

        using var smtpClient = new SmtpClient(_options.Host, _options.Port)
        {
            EnableSsl = true,
            Credentials = new NetworkCredential(_options.Username, _options.Password)
        };

        using var message = new MailMessage
        {
            From = new MailAddress(_options.FromEmail, _options.FromName),
            Subject = "Verify your Flower Shop email address",
            Body = body,
            IsBodyHtml = false
        };
        message.To.Add(new MailAddress(toEmail, toName));

        try
        {
            await smtpClient.SendMailAsync(message, cancellationToken);
            _logger.LogInformation("Verification email sent to {Email}", toEmail);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send verification email to {Email}", toEmail);
            // Surface the error so callers can decide whether to roll back
            throw;
        }
    }
}
