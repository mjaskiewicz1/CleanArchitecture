using Domain.Entities;

namespace Application.Abstractions.Email;

public interface IEmailContentFactory
{
    (string subject, string body) CreateWelcomeSetPasswordEmail(User user);
}