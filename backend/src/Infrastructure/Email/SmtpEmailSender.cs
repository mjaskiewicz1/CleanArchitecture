
using Application.Abstractions.Email;

using MailKit.Net.Smtp;

using Microsoft.Extensions.Options;

using MimeKit;

namespace Infrastructure.Email;

public sealed class SmtpEmailSender(IOptions<EmailSettings> options) : IEmailSender
{
    private readonly EmailSettings _settings = options.Value;

    public async Task SendEmailAsync(string to, string subject, string htmlBody, CancellationToken cancellationToken = default)
    {
        var message = new MimeMessage();
        message.From.Add(MailboxAddress.Parse(_settings.From));
        message.To.Add(MailboxAddress.Parse(to));
        message.Subject = subject;

        var bodyBuilder = new BodyBuilder
        {
            HtmlBody = htmlBody
        };
        message.Body = bodyBuilder.ToMessageBody();

        using var client = new SmtpClient();
        await client.ConnectAsync(_settings.SmtpHost, _settings.SmtpPort, cancellationToken: cancellationToken);

        if (client.Capabilities.HasFlag(SmtpCapabilities.Authentication))
            await client.AuthenticateAsync(_settings.SmtpUser, _settings.SmtpPass, cancellationToken);

        await client.SendAsync(message, cancellationToken);
        await client.DisconnectAsync(true, cancellationToken);
    }
}