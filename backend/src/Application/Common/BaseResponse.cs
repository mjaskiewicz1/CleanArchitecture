namespace Application.Common;


public abstract record BaseResponse
{
    public required ulong Id { get; init; }
}