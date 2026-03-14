using TaskManager.Domain.Models;

namespace TaskManager.Application.Interfaces;

public interface ITokenService
{
    string CreateToken(User user);
}
