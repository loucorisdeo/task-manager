namespace TaskManager.Domain.Models;

public class TaskItem
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? DueDateUtc { get; set; }

    public int TenantId { get; set; }
    public Tenant? Tenant { get; set; }

    public int CreatedByUserId { get; set; }
    public User? CreatedByUser { get; set; }
}