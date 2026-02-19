namespace Domain.Entities.Generic;

public abstract class DbEntity
{
    public ulong Id { get; set; }

    public DateTimeOffset CreatedAtUtc { get; private init; } = DateTimeOffset.UtcNow;
    public void SetId(ulong id) => Id = id;
}