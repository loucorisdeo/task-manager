using System.Collections.Generic;
using System.Threading.Tasks;
using TaskManager.Domain.Models;

namespace TaskManager.Desktop.Services
{
    public class TaskService
    {
        private readonly ApiClient _apiClient;

        public TaskService(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<List<TaskItem>> GetTasksAsync()
        {
            var tasks = await _apiClient.GetAsync<List<TaskItem>>("api/tasks");
            return tasks ?? new List<TaskItem>();
        }

        public async Task CompleteTaskAsync(int taskId)
        {
            await _apiClient.PutAsync($"api/tasks/{taskId}/complete", null);
        }

        public async Task<TaskItem?> CreateTaskAsync(string title, string? description)
        {
            var newTask = new
            {
                title = title,
                description = description
            };

            return await _apiClient.PostAsync<TaskItem>("api/tasks", newTask);
        }

        public async Task DeleteTaskAsync(int taskId)
        {
            await _apiClient.DeleteAsync($"api/tasks/{taskId}");
        }
    }
}