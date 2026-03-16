using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using TaskManager.Application.DTOs;
using TaskManager.Application.Interfaces;
using TaskManager.Domain.Enums;
using TaskManager.Domain.Models;
using TaskManager.Infrastructure.Data;
using TaskManager.Infrastructure.Services;
using Xunit;

namespace TaskManager.Tests.Services
{
    public class AuthServiceTests
    {
        private static AppDbContext CreateDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new AppDbContext(options);
        }

        [Fact]
        public async Task LoginAsync_ReturnsNull_WhenUserDoesNotExist()
        {
            using var dbContext = CreateDbContext();
            var tokenServiceMock = new Mock<ITokenService>();
            var service = new AuthService(dbContext, tokenServiceMock.Object);

            var request = new LoginRequest
            {
                Username = "missing-user",
                Password = "password"
            };

            var result = await service.LoginAsync(request);

            result.Should().BeNull();
            tokenServiceMock.Verify(x => x.CreateToken(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task LoginAsync_ReturnsNull_WhenPasswordIsInvalid()
        {
            using var dbContext = CreateDbContext();

            dbContext.Users.Add(new User
            {
                Id = 1,
                Username = "admin",
                PasswordHash = "correct-password",
                Role = UserRole.Admin,
                TenantId = 1
            });

            await dbContext.SaveChangesAsync();

            var tokenServiceMock = new Mock<ITokenService>();
            var service = new AuthService(dbContext, tokenServiceMock.Object);

            var request = new LoginRequest
            {
                Username = "admin",
                Password = "wrong-password"
            };

            var result = await service.LoginAsync(request);

            result.Should().BeNull();
            tokenServiceMock.Verify(x => x.CreateToken(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task LoginAsync_ReturnsLoginResponse_WhenCredentialsAreValid()
        {
            using var dbContext = CreateDbContext();

            dbContext.Users.Add(new User
            {
                Id = 1,
                Username = "admin",
                PasswordHash = "password",
                Role = UserRole.Admin,
                TenantId = 7
            });

            await dbContext.SaveChangesAsync();

            var tokenServiceMock = new Mock<ITokenService>();
            tokenServiceMock
                .Setup(x => x.CreateToken(It.IsAny<User>()))
                .Returns("fake-jwt-token");

            var service = new AuthService(dbContext, tokenServiceMock.Object);

            var request = new LoginRequest
            {
                Username = "admin",
                Password = "password"
            };

            var result = await service.LoginAsync(request);

            result.Should().NotBeNull();
            result!.Token.Should().Be("fake-jwt-token");
            result.Username.Should().Be("admin");
            result.Role.Should().Be("Admin");
            result.TenantId.Should().Be(7);

            tokenServiceMock.Verify(
                x => x.CreateToken(It.Is<User>(u => u.Username == "admin")),
                Times.Once);
        }
    }
}