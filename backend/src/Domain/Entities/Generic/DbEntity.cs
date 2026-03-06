namespace Domain.Entities.Generic;

public abstract class DbEntity
{
    public ulong Id { get; init; }

    public DateTime CreatedAtUtc { get; private init; } = DateTime.UtcNow;
}