using Microsoft.EntityFrameworkCore;
using TaskManager.Application.DTOs;
using TaskManager.Application.Interfaces;
using TaskManager.Domain.Models;
using TaskManager.Infrastructure.Data;

namespace TaskManager.Infrastructure.Services;

public class TaskService : ITaskService
{
    private readonly AppDbContext _dbContext;

    public TaskService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<TaskDto>> GetTasksAsync(int tenantId)
    {
        return await _dbContext.Tasks
            .AsNoTracking()
            .Where(t => t.TenantId == tenantId)
            .OrderByDescending(t => t.CreatedAtUtc)
            .Select(t => new TaskDto
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                IsCompleted = t.IsCompleted,
                CreatedAtUtc = t.CreatedAtUtc,
                DueDateUtc = t.DueDateUtc
            })
            .ToListAsync();
    }

    public async Task<TaskDto?> GetTaskByIdAsync(int id, int tenantId)
    {
        return await _dbContext.Tasks
            .AsNoTracking()
            .Where(t => t.Id == id && t.TenantId == tenantId)
            .Select(t => new TaskDto
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                IsCompleted = t.IsCompleted,
                CreatedAtUtc = t.CreatedAtUtc,
                DueDateUtc = t.DueDateUtc
            })
            .FirstOrDefaultAsync();
    }

    public async Task<TaskDto> CreateTaskAsync(CreateTaskRequest request, int tenantId, int userId)
    {
        var task = new TaskItem
        {
            Title = request.Title,
            Description = request.Description,
            DueDateUtc = request.DueDateUtc,
            TenantId = tenantId,
            CreatedByUserId = userId,
            CreatedAtUtc = DateTime.UtcNow,
            IsCompleted = false
        };

        _dbContext.Tasks.Add(task);
        await _dbContext.SaveChangesAsync();

        return new TaskDto
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            IsCompleted = task.IsCompleted,
            CreatedAtUtc = task.CreatedAtUtc,
            DueDateUtc = task.DueDateUtc
        };
    }

    public async Task<bool> UpdateTaskAsync(int id, UpdateTaskRequest request, int tenantId)
    {
        var task = await _dbContext.Tasks
            .FirstOrDefaultAsync(t => t.Id == id && t.TenantId == tenantId);

        if (task is null)
            return false;

        task.Title = request.Title;
        task.Description = request.Description;
        task.DueDateUtc = request.DueDateUtc;

        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> CompleteTaskAsync(int id, int tenantId)
    {
        var task = await _dbContext.Tasks
            .FirstOrDefaultAsync(t => t.Id == id && t.TenantId == tenantId);

        if (task is null)
            return false;

        task.IsCompleted = true;
        await _dbContext.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteTaskAsync(int id, int tenantId)
    {
        var task = await _dbContext.Tasks
            .FirstOrDefaultAsync(t => t.Id == id && t.TenantId == tenantId);

        if (task is null)
            return false;

        _dbContext.Tasks.Remove(task);

        await _dbContext.SaveChangesAsync();

        return true;
    }
}
