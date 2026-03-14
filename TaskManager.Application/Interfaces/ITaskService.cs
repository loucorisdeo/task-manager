using TaskManager.Application.DTOs;

namespace TaskManager.Application.Interfaces;

public interface ITaskService
{
    Task<List<TaskDto>> GetTasksAsync(int tenantId);
    Task<TaskDto?> GetTaskByIdAsync(int id, int tenantId);
    Task<TaskDto> CreateTaskAsync(CreateTaskRequest request, int tenantId, int userId);
    Task<bool> UpdateTaskAsync(int id, UpdateTaskRequest request, int tenantId);
    Task<bool> CompleteTaskAsync(int id, int tenantId);
    Task<bool> DeleteTaskAsync(int id, int tenantId);
}
