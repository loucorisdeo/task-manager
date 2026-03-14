using Microsoft.EntityFrameworkCore;
using TaskManager.Application.DTOs;
using TaskManager.Application.Interfaces;
using TaskManager.Infrastructure.Data;

namespace TaskManager.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _dbContext;
    private readonly ITokenService _tokenService;

    public AuthService(AppDbContext dbContext, ITokenService tokenService)
    {
        _dbContext = dbContext;
        _tokenService = tokenService;
    }

    public async Task<LoginResponse?> LoginAsync(LoginRequest request)
    {
        var user = await _dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Username == request.Username);

        if (user is null)
            return null;

        if (user.PasswordHash != request.Password)
            return null;

        var token = _tokenService.CreateToken(user);

        return new LoginResponse
        {
            Token = token,
            Username = user.Username,
            Role = user.Role.ToString(),
            TenantId = user.TenantId
        };
    }
}
