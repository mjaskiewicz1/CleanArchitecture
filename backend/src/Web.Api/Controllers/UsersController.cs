using System.ComponentModel.DataAnnotations;

using Application.Users.Commands.CreateUser;
using Application.Users.Commands.DeleteUser;
using Application.Users.Commands.Login;
using Application.Users.Commands.RefreshToken;
using Application.Users.Commands.Revoke;
using Application.Users.Commands.SetPassword;
using Application.Users.Commands.UpdateUser;
using Application.Users.Dtos;
using Application.Users.Queries.GetAllUser;
using Application.Users.Queries.GetCurrentUser;
using Application.Users.Queries.GetUserById;

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
    #region Authentication

    [HttpPost(EndpointPathMapping.Users.Login)]
    [AllowAnonymous]
    [ProducesResponseType<LoginResponse>(statusCode: StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(statusCode: StatusCodes.Status409Conflict)]
    [ProducesResponseType<ProblemDetails>(statusCode: StatusCodes.Status400BadRequest)]
    [EndpointDescription("Authenticates a user and returns access and refresh tokens.")]
    public async Task<IActionResult> LoginAsync([FromBody, Required] LoginUserCommand command,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);

        return result.ToOkObjectResult();
    }

    [HttpDelete(EndpointPathMapping.Users.Revoke)]
    [EndpointDescription("Revokes all active refresh tokens for the authenticated user.")]
    [ProducesResponseType<ProblemDetails>(statusCode: StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(statusCode: StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> RevokeAsync(CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new RevokeRefreshTokensCommand(), cancellationToken);

        return result.ToOkResult();
    }

    [HttpPost(EndpointPathMapping.Users.RefreshToken)]
    [AllowAnonymous]
    [ProducesResponseType<ProblemDetails>(statusCode: StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(statusCode: StatusCodes.Status403Forbidden)]
    [ProducesResponseType<RefreshTokenResponse>(statusCode: StatusCodes.Status200OK)]
    public async Task<IActionResult> RefreshTokenAsync([FromBody, Required] RefreshTokenCommand command,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);

        return result.ToOkObjectResult();
    }

    [HttpPost(EndpointPathMapping.Users.SetPassword)]
    [AllowAnonymous]
    [EndpointDescription("Sets a new password using a reset token.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SetPasswordAsync([FromBody, Required] SetPasswordCommand command,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);

        return result.NoContentResult();
    }

    [HttpGet(EndpointPathMapping.Users.Me)]
    [ProducesResponseType<UserDetailsResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> MeAsync(CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetCurrentUserQuery(), cancellationToken);

        return result.ToOkObjectResult();
    }

    #endregion

    #region Read

    [HttpGet]
    [Authorize(Policy = PolicyNames.UserRead)]
    [EndpointDescription("Retrieves users.")]
    [ProducesResponseType<IEnumerable<UserResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetAllAsync([FromQuery, Required] GetAllUsersQuery query,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(query, cancellationToken);

        return result.ToOkObjectResult();
    }

    [HttpGet(EndpointPathMapping.ById)]
    [Authorize(Policy = PolicyNames.UserRead)]
    [EndpointDescription("Retrieves the user details by ID.")]
    [ProducesResponseType<UserDetailsResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetByIdAsync([FromRoute, Required] ulong id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetUserByIdQuery(id), cancellationToken);

        return result.ToOkObjectResult();
    }

    #endregion

    #region Create

    [HttpPost]
    [Authorize(Policy = PolicyNames.UserCreate)]
    [EndpointDescription("Creates a new user.")]
    [ProducesResponseType<UserResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateAsync([FromBody, Required] CreateUserCommand command,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return result.ToCreatedAtAction(nameof(GetByIdAsync));
    }

    #endregion

    #region Update

    [HttpPut(EndpointPathMapping.ById)]
    [Authorize(Policy = PolicyNames.UserUpdate)]
    [EndpointDescription("Updates an existing user by ID.")]
    [ProducesResponseType<UserResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateAsync([FromRoute, Required] ulong id,
        [FromBody, Required] UpdateUserCommand command, CancellationToken cancellationToken)
    {
        command.Id = id;
        var result = await mediator.Send(command, cancellationToken);
        return result.ToOkObjectResult();
    }

    #endregion

    #region Delete

    [HttpDelete(EndpointPathMapping.ById)]
    [Authorize(Policy = PolicyNames.UserDelete)]
    [EndpointDescription("Deletes a user by ID.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAsync([FromRoute, Required] ulong id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new DeleteUserCommand(id), cancellationToken);

        return result.NoContentResult();
    }

    #endregion
}