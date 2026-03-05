using System.Text.Json.Serialization;

using Domain.Entities.Generic;

using JetBrains.Annotations;

namespace Application.Common;

[UsedImplicitly(ImplicitUseTargetFlags.Members)]
public abstract record UpdateRequest<TEntity> where TEntity : DbEntity
{
    [JsonIgnore]
    public ulong Id { get; set; }

    public abstract void Update(TEntity entity);
}