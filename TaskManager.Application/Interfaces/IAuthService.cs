using TaskManager.Application.DTOs;

namespace TaskManager.Application.Interfaces;

public interface IAuthService
{
    Task<LoginResponse?> LoginAsync(LoginRequest request);
}
