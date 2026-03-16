using System.Security.Claims;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TaskManager.Api.Controllers;
using TaskManager.Application.DTOs;
using TaskManager.Application.Interfaces;
using Xunit;

namespace TaskManager.Tests.Controllers
{
    public class TasksControllerTests
    {
        private readonly Mock<ITaskService> _taskServiceMock;
        private readonly TasksController _controller;

        public TasksControllerTests()
        {
            _taskServiceMock = new Mock<ITaskService>();
            _controller = new TasksController(_taskServiceMock.Object);
        }

        private void SetUserClaims(params Claim[] claims)
        {
            var user = new ClaimsPrincipal(
                new ClaimsIdentity(claims, "TestAuth"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = user
                }
            };
        }

        [Fact]
        public async Task GetTasks_ReturnsUnauthorized_WhenTenantIdClaimMissing()
        {
            SetUserClaims();

            var result = await _controller.GetTasks();

            var unauthorized = result.Should().BeOfType<UnauthorizedObjectResult>().Subject;
            unauthorized.Value.Should().Be("Missing tenantId claim.");
        }

        [Fact]
        public async Task GetTasks_ReturnsOk_WithTasks_WhenTenantIdClaimExists()
        {
            var tasks = new List<TaskDto>
            {
                new TaskDto { Id = 1, Title = "Task 1", IsCompleted = false },
                new TaskDto { Id = 2, Title = "Task 2", IsCompleted = true }
            };

            SetUserClaims(new Claim("tenantId", "1"));

            _taskServiceMock
                .Setup(s => s.GetTasksAsync(1))
                .ReturnsAsync(tasks);

            var result = await _controller.GetTasks();

            var ok = result.Should().BeOfType<OkObjectResult>().Subject;
            var value = ok.Value.Should().BeAssignableTo<IEnumerable<TaskDto>>().Subject;

            value.Should().HaveCount(2);
            _taskServiceMock.Verify(s => s.GetTasksAsync(1), Times.Once);
        }

        [Fact]
        public async Task GetTask_ReturnsUnauthorized_WhenTenantIdClaimMissing()
        {
            SetUserClaims();

            var result = await _controller.GetTask(1);

            var unauthorized = result.Should().BeOfType<UnauthorizedObjectResult>().Subject;
            unauthorized.Value.Should().Be("Missing tenantId claim.");
        }

        [Fact]
        public async Task GetTask_ReturnsNotFound_WhenTaskDoesNotExist()
        {
            SetUserClaims(new Claim("tenantId", "1"));

            _taskServiceMock
                .Setup(s => s.GetTaskByIdAsync(123, 1))
                .ReturnsAsync((TaskDto?)null);

            var result = await _controller.GetTask(123);

            result.Should().BeOfType<NotFoundResult>();
            _taskServiceMock.Verify(s => s.GetTaskByIdAsync(123, 1), Times.Once);
        }

        [Fact]
        public async Task GetTask_ReturnsOk_WhenTaskExists()
        {
            var task = new TaskDto
            {
                Id = 1,
                Title = "Test Task",
                IsCompleted = false
            };

            SetUserClaims(new Claim("tenantId", "1"));

            _taskServiceMock
                .Setup(s => s.GetTaskByIdAsync(1, 1))
                .ReturnsAsync(task);

            var result = await _controller.GetTask(1);

            var ok = result.Should().BeOfType<OkObjectResult>().Subject;
            var value = ok.Value.Should().BeAssignableTo<TaskDto>().Subject;

            value.Id.Should().Be(1);
            value.Title.Should().Be("Test Task");
        }

        [Fact]
        public async Task CreateTask_ReturnsUnauthorized_WhenRequiredClaimsMissing()
        {
            var request = new CreateTaskRequest
            {
                Title = "New Task",
                Description = "Test description"
            };

            SetUserClaims();

            var result = await _controller.CreateTask(request);

            var unauthorized = result.Should().BeOfType<UnauthorizedObjectResult>().Subject;
            unauthorized.Value.Should().Be("Missing required claims.");
        }

        [Fact]
        public async Task CreateTask_ReturnsCreatedAtAction_WhenSuccessful()
        {
            var request = new CreateTaskRequest
            {
                Title = "New Task",
                Description = "Test description"
            };

            var createdTask = new TaskDto
            {
                Id = 10,
                Title = "New Task",
                Description = "Test description",
                IsCompleted = false
            };

            SetUserClaims(
                new Claim("tenantId", "1"),
                new Claim(ClaimTypes.NameIdentifier, "42"));

            _taskServiceMock
                .Setup(s => s.CreateTaskAsync(request, 1, 42))
                .ReturnsAsync(createdTask);

            var result = await _controller.CreateTask(request);

            var created = result.Should().BeOfType<CreatedAtActionResult>().Subject;
            created.ActionName.Should().Be(nameof(TasksController.GetTask));
            created.RouteValues!["id"].Should().Be(10);
            created.Value.Should().Be(createdTask);

            _taskServiceMock.Verify(s => s.CreateTaskAsync(request, 1, 42), Times.Once);
        }

        [Fact]
        public async Task UpdateTask_ReturnsUnauthorized_WhenTenantIdClaimMissing()
        {
            var request = new UpdateTaskRequest
            {
                Title = "Updated Task",
                Description = "Updated description"
            };

            SetUserClaims();

            var result = await _controller.UpdateTask(1, request);

            var unauthorized = result.Should().BeOfType<UnauthorizedObjectResult>().Subject;
            unauthorized.Value.Should().Be("Missing tenantId claim.");
        }

        [Fact]
        public async Task UpdateTask_ReturnsNotFound_WhenUpdateFails()
        {
            var request = new UpdateTaskRequest
            {
                Title = "Updated Task",
                Description = "Updated description"
            };

            SetUserClaims(new Claim("tenantId", "1"));

            _taskServiceMock
                .Setup(s => s.UpdateTaskAsync(1, request, 1))
                .ReturnsAsync(false);

            var result = await _controller.UpdateTask(1, request);

            result.Should().BeOfType<NotFoundResult>();
            _taskServiceMock.Verify(s => s.UpdateTaskAsync(1, request, 1), Times.Once);
        }

        [Fact]
        public async Task UpdateTask_ReturnsNoContent_WhenSuccessful()
        {
            var request = new UpdateTaskRequest
            {
                Title = "Updated Task",
                Description = "Updated description"
            };

            SetUserClaims(new Claim("tenantId", "1"));

            _taskServiceMock
                .Setup(s => s.UpdateTaskAsync(1, request, 1))
                .ReturnsAsync(true);

            var result = await _controller.UpdateTask(1, request);

            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task CompleteTask_ReturnsUnauthorized_WhenTenantIdClaimMissing()
        {
            SetUserClaims();

            var result = await _controller.CompleteTask(1);

            var unauthorized = result.Should().BeOfType<UnauthorizedObjectResult>().Subject;
            unauthorized.Value.Should().Be("Missing tenantId claim.");
        }

        [Fact]
        public async Task CompleteTask_ReturnsNotFound_WhenCompleteFails()
        {
            SetUserClaims(new Claim("tenantId", "1"));

            _taskServiceMock
                .Setup(s => s.CompleteTaskAsync(1, 1))
                .ReturnsAsync(false);

            var result = await _controller.CompleteTask(1);

            result.Should().BeOfType<NotFoundResult>();
            _taskServiceMock.Verify(s => s.CompleteTaskAsync(1, 1), Times.Once);
        }

        [Fact]
        public async Task CompleteTask_ReturnsNoContent_WhenSuccessful()
        {
            SetUserClaims(new Claim("tenantId", "1"));

            _taskServiceMock
                .Setup(s => s.CompleteTaskAsync(1, 1))
                .ReturnsAsync(true);

            var result = await _controller.CompleteTask(1);

            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task DeleteTask_ReturnsUnauthorized_WhenTenantIdClaimMissing()
        {
            SetUserClaims();

            var result = await _controller.DeleteTask(1);

            var unauthorized = result.Should().BeOfType<UnauthorizedObjectResult>().Subject;
            unauthorized.Value.Should().Be("Missing tenantId claim.");
        }

        [Fact]
        public async Task DeleteTask_ReturnsNotFound_WhenDeleteFails()
        {
            SetUserClaims(new Claim("tenantId", "1"));

            _taskServiceMock
                .Setup(s => s.DeleteTaskAsync(1, 1))
                .ReturnsAsync(false);

            var result = await _controller.DeleteTask(1);

            result.Should().BeOfType<NotFoundResult>();
            _taskServiceMock.Verify(s => s.DeleteTaskAsync(1, 1), Times.Once);
        }

        [Fact]
        public async Task DeleteTask_ReturnsNoContent_WhenSuccessful()
        {
            SetUserClaims(new Claim("tenantId", "1"));

            _taskServiceMock
                .Setup(s => s.DeleteTaskAsync(1, 1))
                .ReturnsAsync(true);

            var result = await _controller.DeleteTask(1);

            result.Should().BeOfType<NoContentResult>();
        }
    }
}