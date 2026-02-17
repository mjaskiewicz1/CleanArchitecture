using System.Net;
using System.Net.Http.Json;

using Application.Users.Commands.Login;
using Application.Users.Commands.RefreshToken;
using Application.Users.Dtos;

using Microsoft.AspNetCore.Mvc;

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
        private const string Logout = EndpointPathMapping.Users.Logout;
        private const string RefreshToken = EndpointPathMapping.Users.RefreshToken;

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
        public async Task LogoutAsync_ValidRequest_ReturnsOk(string login, string password)
        {
            // Arrange
            var client = await WebApplicationFactory.CreateAuthenticatedClientAsync(login, password);

            var url = UsersPath.ToRelativeUri(Logout);

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
        public async Task RefreshTokenAsync_ValidToken_ReturnsOk(
            string login,
            string password)
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

    }
}