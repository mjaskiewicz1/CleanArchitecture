using Domain.Entities.Generic;

using JetBrains.Annotations;

namespace Application.Common;

/// <summary>
/// Defines the contract for mapping domain entities to response DTOs.
/// </summary>
[UsedImplicitly(ImplicitUseTargetFlags.Members)]
public interface IResponse<in TEntity, out TResponse>
    where TEntity : DbEntity
    where TResponse : BaseResponse
{
    /// <summary>
    /// Maps a domain entity to its corresponding response DTO.
    /// Must be implemented by each sealed record that implements this interface.
    /// </summary>
    static abstract TResponse FromEntity(TEntity entity);
}