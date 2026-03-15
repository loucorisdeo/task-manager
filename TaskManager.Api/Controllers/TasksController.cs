using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Application.DTOs;
using TaskManager.Application.Interfaces;

namespace TaskManager.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TasksController : ControllerBase
{
    private readonly ITaskService _taskService;

    public TasksController(ITaskService taskService)
    {
        _taskService = taskService;
    }

    [HttpGet]
    public async Task<IActionResult> GetTasks()
    {
        var tenantIdValue = User.FindFirstValue("tenantId");

        if (string.IsNullOrWhiteSpace(tenantIdValue))
            return Unauthorized("Missing tenantId claim.");

        var tenantId = int.Parse(tenantIdValue);
        var tasks = await _taskService.GetTasksAsync(tenantId);

        return Ok(tasks);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetTask(int id)
    {
        var tenantIdValue = User.FindFirstValue("tenantId");

        if (string.IsNullOrWhiteSpace(tenantIdValue))
            return Unauthorized("Missing tenantId claim.");

        var tenantId = int.Parse(tenantIdValue);
        var task = await _taskService.GetTaskByIdAsync(id, tenantId);

        if (task is null)
            return NotFound();

        return Ok(task);
    }

    [HttpPost]
    public async Task<IActionResult> CreateTask([FromBody] CreateTaskRequest request)
    {
        var tenantIdValue = User.FindFirstValue("tenantId");
        var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrWhiteSpace(tenantIdValue) || string.IsNullOrWhiteSpace(userIdValue))
            return Unauthorized("Missing required claims.");

        var tenantId = int.Parse(tenantIdValue);
        var userId = int.Parse(userIdValue);

        var task = await _taskService.CreateTaskAsync(request, tenantId, userId);

        return CreatedAtAction(nameof(GetTask), new { id = task.Id }, task);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateTask(int id, [FromBody] UpdateTaskRequest request)
    {
        var tenantIdValue = User.FindFirstValue("tenantId");

        if (string.IsNullOrWhiteSpace(tenantIdValue))
            return Unauthorized("Missing tenantId claim.");

        var tenantId = int.Parse(tenantIdValue);
        var updated = await _taskService.UpdateTaskAsync(id, request, tenantId);

        if (!updated)
            return NotFound();

        return NoContent();
    }

    [HttpPut("{id:int}/complete")]
    public async Task<IActionResult> CompleteTask(int id)
    {
        var tenantIdValue = User.FindFirstValue("tenantId");

        if (string.IsNullOrWhiteSpace(tenantIdValue))
            return Unauthorized("Missing tenantId claim.");

        var tenantId = int.Parse(tenantIdValue);
        var updated = await _taskService.CompleteTaskAsync(id, tenantId);

        if (!updated)
            return NotFound();

        return NoContent();
    }


    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteTask(int id)
    {
        var tenantIdValue = User.FindFirstValue("tenantId");

        if (string.IsNullOrWhiteSpace(tenantIdValue))
            return Unauthorized("Missing tenantId claim.");

        var tenantId = int.Parse(tenantIdValue);

        var deleted = await _taskService.DeleteTaskAsync(id, tenantId);

        if (!deleted)
            return NotFound();

        return NoContent();
    }

}
