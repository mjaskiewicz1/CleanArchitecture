namespace Domain.Entities.Generic;

public abstract class DbEntity
{
    public ulong Id { get; init; }

    public DateTimeOffset CreatedAtUtc { get; private init; } = DateTimeOffset.UtcNow;
}