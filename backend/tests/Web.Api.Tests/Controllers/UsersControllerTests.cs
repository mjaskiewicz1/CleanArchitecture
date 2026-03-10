using System.Net;
using System.Net.Http.Json;

using Application.Users.Commands.CreateUser;
using Application.Users.Commands.Login;
using Application.Users.Commands.RefreshToken;
using Application.Users.Commands.SetPassword;
using Application.Users.Commands.UpdateUser;
using Application.Users.Dtos;

using Domain.Entities.Enums;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Web.Api.Tests.Extensions;

namespace Web.Api.Tests.Controllers;

internal class UsersControllerTests
{
    private const string UsersPath = EndpointPathMapping.Users.Base;

    [ClassDataSource<TestWebApplicationFactory>(Shared = SharedType.PerClass)]
    public static TestWebApplicationFactory WebApplicationFactory { get; set; } = null!;

    public class AuthUsers
    {
        private const string Login = EndpointPathMapping.Users.Login;
        private const string Revoke = EndpointPathMapping.Users.Revoke;
        private const string RefreshToken = EndpointPathMapping.Users.RefreshToken;
        private const string Me = EndpointPathMapping.Users.Me;
        private const string SetPassword = EndpointPathMapping.Users.SetPassword;

        [Test]
        [Arguments("fake@fake.pl", "fake")]
        public async Task LoginAsync_InvalidCredentials_ReturnsUnauthorized(string login, string password)
        {
            // Arrange
            var client = WebApplicationFactory.CreateClient();
            var request = new LoginUserCommand(login, password);
            var url = UsersPath.ToRelativeUri(Login);

            // Act
            var response = await client.PostAsJsonAsync(url, request);

            // Assert
            await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.Unauthorized);

            var problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>();
            await Assert.That(problemDetails).IsNotNull();
            await Assert.That(problemDetails!.Title).IsNotNull();
        }

        [Test]
        [Arguments("email", "fake")]
        [Arguments("", "")]
        [Arguments("email@wp.pl", "")]
        [Arguments("email", "password")]
        public async Task LoginAsync_InvalidLoginData_ReturnsBadRequest(string login, string password)
        {
            // Arrange
            var client = WebApplicationFactory.CreateClient();
            var request = new LoginUserCommand(login, password);
            var url = UsersPath.ToRelativeUri(Login);

            // Act
            var response = await client.PostAsJsonAsync(url, request);

            // Assert
            await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.BadRequest);

            var problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>();
            await Assert.That(problemDetails).IsNotNull();
            await Assert.That(problemDetails!.Title).IsNotNull();
        }

        [Test]
        [Arguments("admin@admin.com", "test")]
        public async Task LoginAsync_ValidRequest_ReturnsOk(string login, string password)
        {
            // Arrange
            var client = WebApplicationFactory.CreateClient();
            var request = new LoginUserCommand(login, password);
            var url = UsersPath.ToRelativeUri(Login);

            // Act
            var response = await client.PostAsJsonAsync(url, request);

            // Assert
            await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.OK);
        }

        [Test]
        [Arguments("admin@admin.com", "test")]
        public async Task RevokeAsync_ValidRequest_ReturnsOk(string login, string password)
        {
            // Arrange
            var client = await WebApplicationFactory.CreateAuthenticatedClientAsync(login, password);
            var url = UsersPath.ToRelativeUri(Revoke);

            // Act
            var response = await client.DeleteAsync(url);

            // Assert
            await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.OK);
        }

        [Test]
        [Arguments("invalid-refresh-token")]
        public async Task RefreshTokenAsync_InvalidToken_ReturnsBadRequest(string refreshToken)
        {
            // Arrange
            var client = WebApplicationFactory.CreateClient();
            var request = new RefreshTokenCommand(refreshToken);
            var url = UsersPath.ToRelativeUri(RefreshToken);

            // Act
            var response = await client.PostAsJsonAsync(url, request);

            // Assert
            await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.BadRequest);

            var problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>();
            await Assert.That(problemDetails).IsNotNull();
        }

        [Test]
        [Arguments("admin@admin.com", "test")]
        public async Task RefreshTokenAsync_ValidToken_ReturnsOk(string login, string password)
        {
            // Arrange
            var client = WebApplicationFactory.CreateClient();

            var loginRequest = new LoginUserCommand(login, password);
            var loginUrl = UsersPath.ToRelativeUri(EndpointPathMapping.Users.Login);

            var loginResponse = await client.PostAsJsonAsync(loginUrl, loginRequest);

            await Assert.That(loginResponse.StatusCode)
                .IsEqualTo(HttpStatusCode.OK);

            var loginResult = await loginResponse.Content
                .ReadFromJsonAsync<LoginResponse>();

            await Assert.That(loginResult).IsNotNull();

            var refreshRequest = new RefreshTokenCommand(loginResult!.RefreshToken);
            var refreshUrl = UsersPath.ToRelativeUri(RefreshToken);

            // Act
            var refreshResponse = await client.PostAsJsonAsync(refreshUrl, refreshRequest);

            // Assert
            await Assert.That(refreshResponse.StatusCode).IsEqualTo(HttpStatusCode.OK);

            var refreshResult = await refreshResponse.Content.ReadFromJsonAsync<RefreshTokenResponse>();

            await Assert.That(refreshResult).IsNotNull();
            await Assert.That(refreshResult!.AccessToken).IsNotNull();
            await Assert.That(refreshResult.RefreshToken).IsNotNull();
        }

        [Test]
        [Arguments("admin@admin.com", "test")]
        public async Task GetProfileAsync_AuthenticatedRequest_ReturnsOk(string login, string password)
        {
            // Arrange
            var client = await WebApplicationFactory.CreateAuthenticatedClientAsync(login, password);
            var meUrl = UsersPath.ToRelativeUri(Me);

            // Act
            var response = await client.GetAsync(meUrl);

            // Assert
            await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<UserDetailsResponse>();
            await Assert.That(result).IsNotNull();
            await Assert.That(result!.Email).IsEqualTo(login);
            await Assert.That(result.Permissions).IsNotNull();
        }

        [Test]
        public async Task GetProfileAsync_WithoutAuthentication_ReturnsUnauthorized()
        {
            // Arrange
            var client = WebApplicationFactory.CreateClient();
            var meUrl = UsersPath.ToRelativeUri(Me);

            // Act
            var response = await client.GetAsync(meUrl);

            // Assert
            await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.Unauthorized);
        }

        [Test]
        [Arguments("", "Password123")]
        [Arguments("token", "")]
        [Arguments("", "")]
        public async Task SetPasswordAsync_InvalidData_ReturnsBadRequest(string token, string password)
        {
            // Arrange
            var client = WebApplicationFactory.CreateClient();
            var request = new SetPasswordCommand(token, password);
            var url = UsersPath.ToRelativeUri(SetPassword);

            // Act
            var response = await client.PostAsJsonAsync(url, request);

            // Assert
            await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.BadRequest);

            var problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>();

            await Assert.That(problemDetails).IsNotNull();
            await Assert.That(problemDetails!.Title).IsNotNull();
        }

        [Test]
        public async Task SetPasswordAsync_InvalidToken_ReturnsBadRequest()
        {
            // Arrange
            var client = WebApplicationFactory.CreateClient();
            var request = new SetPasswordCommand("invalid-token", "Password123");
            var url = UsersPath.ToRelativeUri(SetPassword);

            // Act
            var response = await client.PostAsJsonAsync(url, request);

            // Assert
            await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.BadRequest);

            var problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>();

            await Assert.That(problemDetails).IsNotNull();
            await Assert.That(problemDetails!.Title).IsNotNull();
        }

        [Test]
        public async Task SetPasswordAsync_ExpiredToken_ReturnsBadRequest()
        {
            // Arrange
            var user = await WebApplicationFactory.Seeder.CreateUserWithPermissionsAsync();
            var dbContext = WebApplicationFactory.GetDbContext();

            user.PasswordResetToken = "expired-token";
            user.PasswordResetTokenExpiryUtc = DateTime.UtcNow.AddHours(-1);

            await dbContext.SaveChangesAsync();

            var client = WebApplicationFactory.CreateClient();

            var request = new SetPasswordCommand("expired-token", "Password123");

            var url = UsersPath.ToRelativeUri(SetPassword);

            // Act
            var response = await client.PostAsJsonAsync(url, request);

            // Assert
            await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.BadRequest);
        }

        [Test]
        public async Task SetPasswordAsync_ValidToken_ReturnsNoContent()
        {
            // Arrange
            var user = await WebApplicationFactory.Seeder.CreateUserWithPermissionsAsync();
            var dbContext = WebApplicationFactory.GetDbContext();

            user.PasswordResetToken = "valid-token";
            user.PasswordResetTokenExpiryUtc = DateTime.UtcNow.AddHours(1);
            dbContext.Update(user);
            await dbContext.SaveChangesAsync();
            var client = WebApplicationFactory.CreateClient();
            var request = new SetPasswordCommand(user.PasswordResetToken, "Password123");
            var url = UsersPath.ToRelativeUri(SetPassword);

            // Act
            var response = await client.PostAsJsonAsync(url, request);

            // Assert
            await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.NoContent);
            var updatedUser = await dbContext.Users.AsNoTracking().FirstAsync(x => x.Id == user.Id);

            await Assert.That(updatedUser.PasswordHash).IsNotNull();
            await Assert.That(updatedUser.PasswordResetToken).IsNull();
            await Assert.That(updatedUser.PasswordResetTokenExpiryUtc).IsNull();
        }
    }

    public class ReadUsers
    {
        [Test]
        public async Task GetUserAsync_WithoutAuthentication_ReturnsUnauthorized()
        {
            // Arrange
            var client = WebApplicationFactory.CreateClient();
            var url = UsersPath.ToRelativeUri("1");

            // Act
            var response = await client.GetAsync(url);

            // Assert
            await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.Unauthorized);
        }

        [Test]
        [Arguments("admin@admin.com", "test")]
        public async Task GetUserAsync_NonExistentUser_ReturnsNotFound(string login, string password)
        {
            // Arrange
            var client = await WebApplicationFactory.CreateAuthenticatedClientAsync(login, password);
            var url = UsersPath.ToRelativeUri(long.MaxValue.ToString());

            // Act
            var response = await client.GetAsync(url);

            // Assert
            await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.NotFound);
        }

        [Test]
        public async Task GetUserAsync_WithPermission_ReturnsOk()
        {
            // Arrange
            var user = await WebApplicationFactory.Seeder
                .CreateUserWithPermissionsAsync(permissions: [PermissionName.UserRead]);

            var client = await WebApplicationFactory.CreateAuthenticatedClientAsync(user.Email);

            var url = UsersPath.ToRelativeUri(user.Id.ToString());

            // Act
            var response = await client.GetAsync(url);

            // Assert
            await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.OK);
        }

        [Test]
        public async Task GetAllUsersAsync_WithPermission_ReturnsOk()
        {
            var user = await WebApplicationFactory.Seeder.CreateUserWithPermissionsAsync(permissions:
                [PermissionName.UserRead]);

            var client = await WebApplicationFactory.CreateAuthenticatedClientAsync(user.Email);

            // Act
            var response = await client.GetAsync(UsersPath.ToRelativeUri());

            // Assert
            await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.OK);
        }

        [Test]
        public async Task GetUserAsync_WithOtherPermissions_ReturnsForbidden()
        {
            // Arrange
            var permissions = Enum.GetValues<PermissionName>()
                .Where(x => x != PermissionName.UserRead);
            var user = await WebApplicationFactory.Seeder
                .CreateUserWithPermissionsAsync(permissions: permissions);

            var client = await WebApplicationFactory.CreateAuthenticatedClientAsync(user.Email);

            // Act
            var response = await client.GetAsync(UsersPath.ToRelativeUri());

            // Assert
            await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.Forbidden);
        }

        [Test]
        [Arguments(long.MaxValue, 10)]
        public async Task GetUserAsync_TestPaginationReturnEmptyList(long cursor, int take)
        {
            // Arrange
            var client = await WebApplicationFactory.CreateAuthenticatedClientAsync();

            // Act
            var response = await client.GetAsync(UsersPath.ToRelativeUri($"?cursor={cursor}&take={take}"));

            // Assert
            await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<List<UserResponse>>();
            await Assert.That(result).IsNotNull();
            await Assert.That(result).IsEmpty();
        }

        [Test]
        [Arguments(0, 10, "admin@admin.com")]
        [Arguments(0, 0, "admin@admin.com")]
        [Arguments(0, 0, "admin@admin.com")]
        [Arguments(0, 100, "admin@admin.com")]
        public async Task GetUserAsync_TestPaginationReturnUsersWithEmailAndAdminLogin(long cursor, int take,
            string email)
        {
            // Arrange
            var client = await WebApplicationFactory.CreateAuthenticatedClientAsync();

            // Act
            var response =
                await client.GetAsync(UsersPath.ToRelativeUri($"?cursor={cursor}&take={take}&email={email}"));

            // Assert
            await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<List<UserResponse>>();
            await Assert.That(result).IsNotNull();
            await Assert.That(result).IsNotEmpty();
            await Assert.That(result!.Count).IsEqualTo(1);
            await Assert.That(result[0].Email).IsEqualTo(email);
        }
    }

    public class CreateUser
    {
        [Test]
        public async Task CreateUserAsync_WithoutAuthentication_ReturnsUnauthorized()
        {
            // Arrange
            var client = WebApplicationFactory.CreateClient();
            var request = new CreateUserCommand("John", "Doe", "newuser@test.com", []);
            var url = UsersPath.ToRelativeUri();

            // Act
            var response = await client.PostAsJsonAsync(url, request);

            // Assert
            await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.Unauthorized);
        }

        [Test]
        public async Task CreateUserAsync_WithPermission_ReturnsCreated()
        {
            // Arrange
            var creator = await WebApplicationFactory.Seeder.CreateUserWithPermissionsAsync(permissions:
                [PermissionName.UserCreate]);
            var client = await WebApplicationFactory.CreateAuthenticatedClientAsync(creator.Email);
            var request = new CreateUserCommand("John", "Doe", "newuser@test.com", []);
            var url = UsersPath.ToRelativeUri();

            // Act
            var response = await client.PostAsJsonAsync(url, request);

            // Assert
            await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.Created);

            var dbContext = WebApplicationFactory.GetDbContext();
            var createdUser = await dbContext.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            await Assert.That(createdUser).IsNotNull();
            await Assert.That(createdUser!.FirstName).IsEqualTo(request.FirstName);
            await Assert.That(createdUser.LastName).IsEqualTo(request.LastName);
            await Assert.That(createdUser.Email).IsEqualTo(request.Email);
        }

        [Test]
        public async Task CreateUserAsync_WithOtherPermissions_ReturnsForbidden()
        {
            // Arrange
            var permissions = Enum.GetValues<PermissionName>().Where(x => x != PermissionName.UserCreate);
            var user = await WebApplicationFactory.Seeder.CreateUserWithPermissionsAsync(permissions: permissions);
            var client = await WebApplicationFactory.CreateAuthenticatedClientAsync(user.Email);
            var request = new CreateUserCommand("John", "Doe", "newuser@test.com", []);
            var url = UsersPath.ToRelativeUri();

            // Act
            var response = await client.PostAsJsonAsync(url, request);

            // Assert
            await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.Forbidden);
        }

        [Test]
        [Arguments("", "Doe", "email@test.com")]
        [Arguments("John", "", "email@test.com")]
        [Arguments("John", "Doe", "invalid-email")]
        [Arguments("", "", "")]
        public async Task CreateUserAsync_InvalidData_ReturnsBadRequest(string firstName, string lastName, string email)
        {
            // Arrange
            var creator = await WebApplicationFactory.Seeder.CreateUserWithPermissionsAsync(permissions:
                [PermissionName.UserCreate]);
            var client = await WebApplicationFactory.CreateAuthenticatedClientAsync(creator.Email);
            var request = new CreateUserCommand(firstName, lastName, email, []);
            var url = UsersPath.ToRelativeUri();

            // Act
            var response = await client.PostAsJsonAsync(url, request);

            // Assert
            await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.BadRequest);
            var problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>();
            await Assert.That(problemDetails).IsNotNull();
            await Assert.That(problemDetails!.Title).IsNotNull();
        }

        [Test]
        public async Task CreateUserAsync_DuplicateEmail_ReturnsConflict()
        {
            // Arrange
            var creator = await WebApplicationFactory.Seeder.CreateUserWithPermissionsAsync(permissions:
                [PermissionName.UserCreate]);
            var existingUser = await WebApplicationFactory.Seeder.CreateUserWithPermissionsAsync("existing@test.com");
            var client = await WebApplicationFactory.CreateAuthenticatedClientAsync(creator.Email);
            var request = new CreateUserCommand("John", "Doe", existingUser.Email, []);
            var url = UsersPath.ToRelativeUri();

            // Act
            var response = await client.PostAsJsonAsync(url, request);

            // Assert
            await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.Conflict);
        }
    }

    public class UpdateUser
    {
        [Test]
        public async Task UpdateUserAsync_WithoutAuthentication_ReturnsUnauthorized()
        {
            // Arrange
            var client = WebApplicationFactory.CreateClient();
            var request = new UpdateUserCommand("John", "Doe", "updated@test.com", []);
            var url = UsersPath.ToRelativeUri("2");

            // Act
            var response = await client.PutAsJsonAsync(url, request);

            // Assert
            await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.Unauthorized);
        }

        [Test]
        public async Task UpdateUserAsync_WithPermission_ReturnsOk()
        {
            // Arrange
            var user = await WebApplicationFactory.Seeder.CreateUserWithPermissionsAsync(permissions:
                [PermissionName.UserUpdate]);
            var client = await WebApplicationFactory.CreateAuthenticatedClientAsync(user.Email);
            var request = new UpdateUserCommand("NewFirst", "NewLast", "newemail@test.com", [1, 2]);
            var url = UsersPath.ToRelativeUri("2");

            // Act
            var response = await client.PutAsJsonAsync(url, request);

            // Assert
            await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.OK);

            var dbContext = WebApplicationFactory.GetDbContext();
            var updatedUser = await dbContext.Users.Include(x => x.UserPermissions).FirstOrDefaultAsync(u => u.Id == 2);
            await Assert.That(updatedUser).IsNotNull();
            await Assert.That(updatedUser!.FirstName).IsEqualTo(request.FirstName);
            await Assert.That(updatedUser.LastName).IsEqualTo(request.LastName);
            await Assert.That(updatedUser.Email).IsEqualTo(request.Email);
            var actualPermissions = updatedUser.UserPermissions.Select(p => p.PermissionId).ToHashSet();
            await Assert.That(actualPermissions).IsEquivalentTo(request.PermissionIds);
        }

        [Test]
        public async Task UpdateUserAsync_WithOtherPermissions_ReturnsForbidden()
        {
            // Arrange
            var permissions = Enum.GetValues<PermissionName>().Where(x => x != PermissionName.UserUpdate);
            var user = await WebApplicationFactory.Seeder.CreateUserWithPermissionsAsync(permissions: permissions);
            var client = await WebApplicationFactory.CreateAuthenticatedClientAsync(user.Email);
            var request = new UpdateUserCommand("John", "Doe", "updated@test.com", []);
            var url = UsersPath.ToRelativeUri("2");

            // Act
            var response = await client.PutAsJsonAsync(url, request);

            // Assert
            await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.Forbidden);
        }

        [Test]
        [Arguments("", "Doe", "email@test.com")]
        [Arguments("John", "", "email@test.com")]
        [Arguments("John", "Doe", "invalid-email")]
        [Arguments("", "", "")]
        public async Task UpdateUserAsync_InvalidData_ReturnsBadRequest(string firstName, string lastName, string email)
        {
            // Arrange
            var client = await WebApplicationFactory.CreateAuthenticatedClientAsync();
            var request = new UpdateUserCommand(firstName, lastName, email, []);
            var url = UsersPath.ToRelativeUri("2");

            // Act
            var response = await client.PutAsJsonAsync(url, request);

            // Assert
            await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.BadRequest);
            var problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>();
            await Assert.That(problemDetails).IsNotNull();
            await Assert.That(problemDetails!.Title).IsNotNull();
        }

        [Test]
        public async Task UpdateUserAsync_DuplicateEmail_ReturnsConflict()
        {
            // Arrange
            var client = await WebApplicationFactory.CreateAuthenticatedClientAsync();
            var request = new UpdateUserCommand("John", "Doe", "admin@admin.com", []);
            var url = UsersPath.ToRelativeUri("2");

            // Act
            var response = await client.PutAsJsonAsync(url, request);

            // Assert
            await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.Conflict);
        }

        [Test]
        public async Task UpdateUserAsync_NonExistentUser_ReturnsNotFound()
        {
            // Arrange
            var client = await WebApplicationFactory.CreateAuthenticatedClientAsync();
            var request = new UpdateUserCommand("John", "Doe", "updated@test.com", []);
            var url = UsersPath.ToRelativeUri(long.MaxValue.ToString());

            // Act
            var response = await client.PutAsJsonAsync(url, request);

            // Assert
            await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.NotFound);
        }
    }

    public class DeleteUser
    {
        [Test]
        public async Task DeleteUserAsync_WithoutAuthentication_ReturnsUnauthorized()
        {
            // Arrange
            var client = WebApplicationFactory.CreateClient();
            var url = UsersPath.ToRelativeUri("2");

            // Act
            var response = await client.DeleteAsync(url);

            // Assert
            await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.Unauthorized);
        }

        [Test]
        [Arguments(3)]
        [Arguments(long.MaxValue)]
        public async Task DeleteUserAsync_WithPermission_ReturnsNoContent(long userIdToDelete)
        {
            // Arrange

            var client = await WebApplicationFactory.CreateAuthenticatedClientAsync();

            var url = UsersPath.ToRelativeUri(userIdToDelete.ToString());

            // Act
            var response = await client.DeleteAsync(url);

            // Assert
            await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.NoContent);
            var dbContext = WebApplicationFactory.GetDbContext();
            await Assert.That(await dbContext.Users.AnyAsync(u => u.Id == (ulong)userIdToDelete)).IsFalse();
        }

        [Test]
        public async Task DeleteUserAsync_WithOtherPermissions_ReturnsForbidden()
        {
            // Arrange
            var permissions = Enum.GetValues<PermissionName>().Where(x => x != PermissionName.UserDelete);
            var user = await WebApplicationFactory.Seeder.CreateUserWithPermissionsAsync(permissions: permissions);
            var client = await WebApplicationFactory.CreateAuthenticatedClientAsync(user.Email);
            var url = UsersPath.ToRelativeUri("2");

            // Act
            var response = await client.DeleteAsync(url);

            // Assert
            await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.Forbidden);
        }
    }
}