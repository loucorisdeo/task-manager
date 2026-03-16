using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TaskManager.Application.DTOs;
using TaskManager.Domain.Models;
using TaskManager.Infrastructure.Data;
using TaskManager.Infrastructure.Services;
using Xunit;

namespace TaskManager.Tests.Services
{
    public class TaskServiceTests
    {
        private static AppDbContext CreateDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new AppDbContext(options);
        }

        [Fact]
        public async Task GetTasksAsync_ReturnsOnlyTasksForTenant()
        {
            using var dbContext = CreateDbContext();

            dbContext.Tasks.AddRange(
                new TaskItem
                {
                    Id = 1,
                    Title = "Tenant 1 Task",
                    TenantId = 1,
                    CreatedAtUtc = new DateTime(2026, 1, 1),
                    IsCompleted = false
                },
                new TaskItem
                {
                    Id = 2,
                    Title = "Tenant 2 Task",
                    TenantId = 2,
                    CreatedAtUtc = new DateTime(2026, 1, 2),
                    IsCompleted = false
                });

            await dbContext.SaveChangesAsync();

            var service = new TaskService(dbContext);

            var result = await service.GetTasksAsync(1);

            result.Should().HaveCount(1);
            result[0].Title.Should().Be("Tenant 1 Task");
        }

        [Fact]
        public async Task GetTasksAsync_ReturnsTasksOrderedByCreatedAtUtcDescending()
        {
            using var dbContext = CreateDbContext();

            dbContext.Tasks.AddRange(
                new TaskItem
                {
                    Id = 1,
                    Title = "Older Task",
                    TenantId = 1,
                    CreatedAtUtc = new DateTime(2026, 1, 1),
                    IsCompleted = false
                },
                new TaskItem
                {
                    Id = 2,
                    Title = "Newer Task",
                    TenantId = 1,
                    CreatedAtUtc = new DateTime(2026, 1, 2),
                    IsCompleted = false
                });

            await dbContext.SaveChangesAsync();

            var service = new TaskService(dbContext);

            var result = await service.GetTasksAsync(1);

            result.Should().HaveCount(2);
            result[0].Title.Should().Be("Newer Task");
            result[1].Title.Should().Be("Older Task");
        }

        [Fact]
        public async Task GetTaskByIdAsync_ReturnsTask_WhenIdAndTenantMatch()
        {
            using var dbContext = CreateDbContext();

            dbContext.Tasks.Add(new TaskItem
            {
                Id = 1,
                Title = "Task 1",
                Description = "Desc",
                TenantId = 1,
                CreatedAtUtc = new DateTime(2026, 1, 1),
                IsCompleted = false
            });

            await dbContext.SaveChangesAsync();

            var service = new TaskService(dbContext);

            var result = await service.GetTaskByIdAsync(1, 1);

            result.Should().NotBeNull();
            result!.Id.Should().Be(1);
            result.Title.Should().Be("Task 1");
        }

        [Fact]
        public async Task GetTaskByIdAsync_ReturnsNull_WhenTenantDoesNotMatch()
        {
            using var dbContext = CreateDbContext();

            dbContext.Tasks.Add(new TaskItem
            {
                Id = 1,
                Title = "Task 1",
                TenantId = 1,
                CreatedAtUtc = new DateTime(2026, 1, 1),
                IsCompleted = false
            });

            await dbContext.SaveChangesAsync();

            var service = new TaskService(dbContext);

            var result = await service.GetTaskByIdAsync(1, 2);

            result.Should().BeNull();
        }

        [Fact]
        public async Task CreateTaskAsync_CreatesTaskWithCorrectValues()
        {
            using var dbContext = CreateDbContext();
            var service = new TaskService(dbContext);

            var request = new CreateTaskRequest
            {
                Title = "New Task",
                Description = "Test description",
                DueDateUtc = new DateTime(2026, 2, 1)
            };

            var result = await service.CreateTaskAsync(request, tenantId: 3, userId: 42);

            result.Should().NotBeNull();
            result.Title.Should().Be("New Task");
            result.Description.Should().Be("Test description");
            result.IsCompleted.Should().BeFalse();
            result.DueDateUtc.Should().Be(new DateTime(2026, 2, 1));

            var savedTask = await dbContext.Tasks.SingleAsync();

            savedTask.Title.Should().Be("New Task");
            savedTask.Description.Should().Be("Test description");
            savedTask.TenantId.Should().Be(3);
            savedTask.CreatedByUserId.Should().Be(42);
            savedTask.IsCompleted.Should().BeFalse();
        }

        [Fact]
        public async Task UpdateTaskAsync_ReturnsFalse_WhenTaskNotFound()
        {
            using var dbContext = CreateDbContext();
            var service = new TaskService(dbContext);

            var request = new UpdateTaskRequest
            {
                Title = "Updated",
                Description = "Updated desc",
                DueDateUtc = new DateTime(2026, 3, 1)
            };

            var result = await service.UpdateTaskAsync(999, request, 1);

            result.Should().BeFalse();
        }

        [Fact]
        public async Task CompleteTaskAsync_MarksTaskCompleted_WhenTaskExists()
        {
            using var dbContext = CreateDbContext();

            dbContext.Tasks.Add(new TaskItem
            {
                Id = 1,
                Title = "Task 1",
                TenantId = 1,
                CreatedAtUtc = new DateTime(2026, 1, 1),
                IsCompleted = false
            });

            await dbContext.SaveChangesAsync();

            var service = new TaskService(dbContext);

            var result = await service.CompleteTaskAsync(1, 1);

            result.Should().BeTrue();

            var updatedTask = await dbContext.Tasks.SingleAsync(t => t.Id == 1);
            updatedTask.IsCompleted.Should().BeTrue();
        }

        [Fact]
        public async Task DeleteTaskAsync_RemovesTask_WhenTaskExists()
        {
            using var dbContext = CreateDbContext();

            dbContext.Tasks.Add(new TaskItem
            {
                Id = 1,
                Title = "Task 1",
                TenantId = 1,
                CreatedAtUtc = new DateTime(2026, 1, 1),
                IsCompleted = false
            });

            await dbContext.SaveChangesAsync();

            var service = new TaskService(dbContext);

            var result = await service.DeleteTaskAsync(1, 1);

            result.Should().BeTrue();
            dbContext.Tasks.Should().BeEmpty();
        }
    }
}