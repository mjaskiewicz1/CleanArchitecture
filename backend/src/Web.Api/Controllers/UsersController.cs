using System.ComponentModel.DataAnnotations;

using Application.Users.Commands.Login;
using Application.Users.Commands.RefreshToken;
using Application.Users.Commands.Revoke;
using Application.Users.Dtos;
using Application.Users.Queries;

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

    [HttpDelete(EndpointPathMapping.Users.Revoke)]
    [EndpointDescription("Revokes all active refresh tokens for the authenticated user.")]
    [ProducesResponseType<ProblemDetails>(statusCode: StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(statusCode: StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> RevokeAsync()
    {
        var result = await mediator.Send(new RevokeRefreshTokensCommand());

        return result.ToActionResult();
    }

    [HttpPost(EndpointPathMapping.Users.RefreshToken)]
    [AllowAnonymous]
    [ProducesResponseType<ProblemDetails>(statusCode: StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(statusCode: StatusCodes.Status403Forbidden)]
    [ProducesResponseType<RefreshTokenResponse>(statusCode: StatusCodes.Status200OK)]
    public async Task<IActionResult> RefreshTokenAsync([FromBody, Required] RefreshTokenCommand command)
    {
        var result = await mediator.Send(command);

        return result.ToActionResult();
    }
    [HttpGet(EndpointPathMapping.Users.Me)]
    [EndpointDescription("Retrieves the user details for the authenticated user.")]
    [ProducesResponseType<UserProfileResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> MeAsync()
    {
        var result = await mediator.Send(new GetCurrentUserQuery());

        return result.ToActionResult();
    }

}