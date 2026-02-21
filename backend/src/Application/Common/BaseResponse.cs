using System.Text.Json.Serialization;

using JetBrains.Annotations;

namespace Application.Common;

[UsedImplicitly(ImplicitUseTargetFlags.Members)]
public abstract record BaseResponse
{
    [JsonPropertyOrder(int.MinValue)]
    public required ulong Id { get; init; }

    [JsonPropertyOrder(int.MaxValue)]
    public required DateTimeOffset CreatedAtUtc { get; init; }
}