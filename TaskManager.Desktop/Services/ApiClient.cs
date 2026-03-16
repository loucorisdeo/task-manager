using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace TaskManager.Desktop.Services
{
    public class ApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;

        public ApiClient()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7015/"); 

            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        public void SetToken(string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }

        public async Task<T?> GetAsync<T>(string endpoint)
        {
            var response = await _httpClient.GetAsync(endpoint);

            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<T>(json, _jsonOptions);
        }

        public async Task<T?> PostAsync<T>(string endpoint, object data)
        {
            var json = JsonSerializer.Serialize(data);

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(endpoint, content);

            response.EnsureSuccessStatusCode();

            var responseJson = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<T>(responseJson, _jsonOptions);
        }

        public async Task PutAsync(string endpoint, object? data)
        {
            HttpContent? content = null;

            if (data != null)
            {
                var json = JsonSerializer.Serialize(data);
                content = new StringContent(json, Encoding.UTF8, "application/json");
            }

            var response = await _httpClient.PutAsync(endpoint, content);

            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteAsync(string endpoint)
        {
            var response = await _httpClient.DeleteAsync(endpoint);

            response.EnsureSuccessStatusCode();
        }
    }
}