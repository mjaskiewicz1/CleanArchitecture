using Application.Users.Dtos;

using Domain.Shared;

using MediatR;

namespace Application.Users.Commands.RefreshToken;

public record RefreshTokenCommand(string RefreshToken) : IRequest<Result<RefreshTokenResponse>>;