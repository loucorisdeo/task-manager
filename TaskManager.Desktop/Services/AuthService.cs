using TaskManager.Application.DTOs;
using TaskManager.Domain.Models;

namespace TaskManager.Desktop.Services
{
    public class AuthService
    {
        private readonly ApiClient _apiClient;

        public AuthService(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<LoginResponse?> LoginAsync(string username, string password)
        {
            var request = new
            {
                username = username,
                password = password
            };

            return await _apiClient.PostAsync<LoginResponse>(
                "api/auth/login",
                request
            );
        }
    }
}