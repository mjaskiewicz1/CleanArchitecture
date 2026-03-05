namespace Application.Abstractions.Email;

public interface IEmailSender
{
    /// <summary>
    /// Asynchronously sends an email with the specified recipient, subject, and HTML body.
    /// </summary>
    /// <param name="to"><see cref="string"/>: The email address of the recipient.</param>
    /// <param name="subject"><see cref="string"/>: The subject line of the email.</param>
    /// <param name="htmlBody"><see cref="string"/>: The HTML content of the email body.</param>
    /// <param name="cancellationToken"></param>
    /// <returns><see cref="Task"/></returns>
    Task SendEmailAsync(string to, string subject, string htmlBody, CancellationToken cancellationToken = default);
}