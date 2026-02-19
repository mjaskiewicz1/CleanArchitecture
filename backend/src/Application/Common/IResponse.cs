using Domain.Entities.Generic;

namespace Application.Common;

public interface IResponse<in TEntity, out TResponse>
    where TEntity : DbEntity
    where TResponse : BaseResponse
{
    static abstract TResponse FromEntity(TEntity entity);
}