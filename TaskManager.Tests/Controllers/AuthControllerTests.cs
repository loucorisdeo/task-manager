using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TaskManager.Api.Controllers;
using TaskManager.Application.DTOs;
using TaskManager.Application.Interfaces;
using Xunit;

namespace TaskManager.Tests.Controllers
{
    public class AuthControllerTests
    {
        private readonly Mock<IAuthService> _authServiceMock;
        private readonly AuthController _controller;

        public AuthControllerTests()
        {
            _authServiceMock = new Mock<IAuthService>();
            _controller = new AuthController(_authServiceMock.Object);
        }

        [Fact]
        public async Task Login_ReturnsOk_WhenCredentialsAreValid()
        {
            // Arrange
            var request = new LoginRequest
            {
                Username = "admin",
                Password = "password"
            };

            var response = new LoginResponse
            {
                Username = "admin",
                Role = "Admin",
                Token = "fake-jwt-token"
            };

            _authServiceMock
                .Setup(s => s.LoginAsync(request))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.Login(request);

            // Assert
            var ok = result.Should().BeOfType<OkObjectResult>().Subject;
            var value = ok.Value.Should().BeAssignableTo<LoginResponse>().Subject;

            value.Username.Should().Be("admin");
            value.Role.Should().Be("Admin");
            value.Token.Should().Be("fake-jwt-token");

            _authServiceMock.Verify(s => s.LoginAsync(request), Times.Once);
        }

        [Fact]
        public async Task Login_ReturnsUnauthorized_WhenCredentialsAreInvalid()
        {
            // Arrange
            var request = new LoginRequest
            {
                Username = "baduser",
                Password = "badpassword"
            };

            _authServiceMock
                .Setup(s => s.LoginAsync(request))
                .ReturnsAsync((LoginResponse?)null);

            // Act
            var result = await _controller.Login(request);

            // Assert
            var unauthorized = result.Should().BeOfType<UnauthorizedObjectResult>().Subject;

            unauthorized.Value.Should().BeEquivalentTo(new
            {
                message = "Invalid username or password"
            });

            _authServiceMock.Verify(s => s.LoginAsync(request), Times.Once);
        }
    }
}