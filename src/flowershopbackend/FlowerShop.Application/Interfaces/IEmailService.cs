namespace FlowerShop.Application.Interfaces;

/// <summary>
/// Sends transactional emails to customers.
/// </summary>
public interface IEmailService
{
    /// <summary>
    /// Sends an email verification link to the specified customer.
    /// </summary>
    /// <param name="toEmail">Recipient email address.</param>
    /// <param name="toName">Recipient display name.</param>
    /// <param name="token">The raw (un-hashed) verification token to embed in the link.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task SendVerificationEmailAsync(string toEmail, string toName, string token, CancellationToken cancellationToken = default);
}
