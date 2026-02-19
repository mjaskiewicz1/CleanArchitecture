using System.Text.Json.Serialization;

namespace Application.Common;

public abstract record BaseResponse
{
    [JsonPropertyOrder(int.MinValue)]
    public required ulong Id { get; init; }
}