using JetBrains.Annotations;

namespace Application.Abstractions.Email;

[UsedImplicitly(ImplicitUseTargetFlags.Members)]
public class EmailSettings
{
    public const string SectionName = "Email";

    public required string SmtpHost { get; init; }
    public required int SmtpPort { get; init; }
    public required string SmtpUser { get; init; }
    public required string SmtpPass { get; init; }
    public required string From { get; init; }
    public required Uri FrontendBaseUrl { get; init; }
}