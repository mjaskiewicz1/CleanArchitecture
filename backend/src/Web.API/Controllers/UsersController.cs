using System.ComponentModel.DataAnnotations;

using Application.Abstractions.Authentication;
using Application.Users.Commands.Login;
using Application.Users.Commands.RefreshToken;
using Application.Users.Commands.Revoke;
using Application.Users.Dtos;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Web.Api.Extensions;


namespace Web.Api.Controllers;

[ApiController]
[Authorize]
[Route(EndpointPathMapping.Users.Base)]
public class UsersController(IMediator mediator) : ControllerBase
{
    [HttpPost(EndpointPathMapping.Users.Login)]
    [AllowAnonymous]
    [ProducesResponseType<LoginResponse>(statusCode: StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(statusCode: StatusCodes.Status400BadRequest)]
    [EndpointDescription("Authenticates a user and returns access and refresh tokens.")]
    public async Task<IActionResult> LoginAsync([FromBody, Required] LoginUserCommand command)
    {
        var result = await mediator.Send(command);

        return result.ToActionResult();
    }

    [EndpointDescription("Revokes all active refresh tokens for the authenticated user.")]
    [HttpDelete(EndpointPathMapping.Users.Revoke)]
    [ProducesResponseType<ProblemDetails>(statusCode: StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(statusCode: StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> RevokeAsync()
    {
        var result = await mediator.Send(new RevokeRefreshTokensCommand());

        return result.ToActionResult();
    }

    [AllowAnonymous]
    [HttpPost(EndpointPathMapping.Users.RefreshToken)]
    [ProducesResponseType<ProblemDetails>(statusCode: StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(statusCode: StatusCodes.Status403Forbidden)]
    [ProducesResponseType<RefreshTokenResponse>(statusCode: StatusCodes.Status200OK)]
    public async Task<IActionResult> RefreshToken([FromBody, Required] RefreshTokenCommand command)
    {
        var result = await mediator.Send(command);

        return result.ToActionResult();
    }
}