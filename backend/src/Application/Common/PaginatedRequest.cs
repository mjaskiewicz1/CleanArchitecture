using System.Text.Json.Serialization;

namespace Application.Common;

public abstract record PaginatedRequest
{
    private const uint MaxTakeAmount = 100;
    private const uint MinTakeAmount = 1;

    [JsonPropertyOrder(int.MaxValue - 1)]
    public ulong Cursor { get; init; }

    [JsonPropertyOrder(int.MaxValue)]
    public uint TakeAmount
    {
        get;
        init => field = Math.Clamp(value, MinTakeAmount, MaxTakeAmount);
    } = 20;
}