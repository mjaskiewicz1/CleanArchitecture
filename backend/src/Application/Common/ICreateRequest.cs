using Domain.Entities.Generic;

using JetBrains.Annotations;

namespace Application.Common;

[UsedImplicitly(ImplicitUseTargetFlags.Members)]
public interface ICreateRequest<out TEntity> where TEntity : DbEntity
{
    public TEntity ToEntity();
}