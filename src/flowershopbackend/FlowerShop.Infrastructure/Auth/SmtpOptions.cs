namespace FlowerShop.Infrastructure.Auth;

/// <summary>
/// Configuration options for the SMTP email sender.
/// Bind from the <c>"Smtp"</c> section in <c>appsettings.json</c>.
/// </summary>
public class SmtpOptions
{
    /// <summary>SMTP server hostname or IP address.</summary>
    public string Host { get; set; } = string.Empty;

    /// <summary>SMTP server port. Defaults to 587 (STARTTLS).</summary>
    public int Port { get; set; } = 587;

    /// <summary>SMTP authentication username.</summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>SMTP authentication password.</summary>
    public string Password { get; set; } = string.Empty;

    /// <summary>Email address shown in the From header.</summary>
    public string FromEmail { get; set; } = string.Empty;

    /// <summary>Display name shown in the From header.</summary>
    public string FromName { get; set; } = "Flower Shop";

    /// <summary>
    /// Base URL of the front-end SPA. Used to build the email verification link.
    /// Defaults to <c>http://localhost:5173</c>.
    /// </summary>
    public string FrontendBaseUrl { get; set; } = "http://localhost:5173";
}
