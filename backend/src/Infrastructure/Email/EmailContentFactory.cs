using Application.Abstractions.Email;

using Domain.Entities;

using Microsoft.Extensions.Options;

namespace Infrastructure.Email;

public class EmailContentFactory(IOptions<EmailSettings> options) : IEmailContentFactory
{
    public (string subject, string body) CreateWelcomeSetPasswordEmail(User user)
    {
        var resetLink = new Uri(options.Value.FrontendBaseUrl, $"set-password?token={user.PasswordResetToken}");
        var body = $"""
                    <html>
                      <body style='font-family: Arial, sans-serif; line-height: 1.5; color: #333;'>
                        <p>Dzień dobry {user.FirstName} {user.LastName},</p>
                        <p>Witamy w systemie naszej firmy!</p>
                        <p>Zanim zalogujesz się po raz pierwszy, musisz ustawić swoje hasło.</p>
                        <p style='text-align: center; margin: 30px 0;'>
                          <a href='{resetLink}' 
                             style='background-color: #007bff; color: white; padding: 12px 24px; text-decoration: none; border-radius: 5px; display: inline-block;'>
                            Ustaw hasło
                          </a>
                        </p>
                        <p>Jeśli przycisk nie działa, skopiuj i wklej poniższy link do przeglądarki:</p>
                        <p style='word-break: break-all;'><a href='{resetLink}'>{resetLink}</a></p>
                        <p>Dziękujemy,<br/>Zespół IT</p>
                      </body>
                    </html>
                    """;

        return ("Witamy w firmie – ustaw swoje hasło", body);
    }
}