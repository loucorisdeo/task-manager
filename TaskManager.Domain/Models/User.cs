using TaskManager.Domain.Enums;

namespace TaskManager.Domain.Models;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public UserRole Role { get; set; }

    public int TenantId { get; set; }
    public Tenant? Tenant { get; set; }

    public ICollection<TaskItem> CreatedTasks { get; set; } = new List<TaskItem>();
}