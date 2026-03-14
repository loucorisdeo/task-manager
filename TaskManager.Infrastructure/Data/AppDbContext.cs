using Microsoft.EntityFrameworkCore;
using TaskManager.Domain.Enums;
using TaskManager.Domain.Models;

namespace TaskManager.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<User> Users => Set<User>();
    public DbSet<TaskItem> Tasks => Set<TaskItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Tenant>()
            .HasMany(t => t.Users)
            .WithOne(u => u.Tenant)
            .HasForeignKey(u => u.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Tenant>()
            .HasMany(t => t.Tasks)
            .WithOne(t => t.Tenant)
            .HasForeignKey(t => t.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<User>()
            .HasMany(u => u.CreatedTasks)
            .WithOne(t => t.CreatedByUser)
            .HasForeignKey(t => t.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<User>()
            .HasIndex(u => new { u.Username, u.TenantId })
            .IsUnique();

        modelBuilder.Entity<TaskItem>()
            .HasIndex(t => new { t.TenantId, t.IsCompleted });

        modelBuilder.Entity<Tenant>().HasData(
            new Tenant
            {
                Id = 1,
                Name = "Tenant A"
            },
            new Tenant
            {
                Id = 2,
                Name = "Tenant B"
            }
        );

        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = 1,
                Username = "adminA",
                PasswordHash = "Password123!",
                Role = UserRole.Admin,
                TenantId = 1
            },
            new User
            {
                Id = 2,
                Username = "userA",
                PasswordHash = "Password123!",
                Role = UserRole.User,
                TenantId = 1
            },
            new User
            {
                Id = 3,
                Username = "adminB",
                PasswordHash = "Password123!",
                Role = UserRole.Admin,
                TenantId = 2
            },
            new User
            {
                Id = 4,
                Username = "userB",
                PasswordHash = "Password123!",
                Role = UserRole.User,
                TenantId = 2
            }
        );

        modelBuilder.Entity<TaskItem>().HasData(
            new TaskItem
            {
                Id = 1,
                Title = "Tenant A Task 1",
                Description = "Initial seeded task",
                IsCompleted = false,
                CreatedAtUtc = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                TenantId = 1,
                CreatedByUserId = 1
            },
            new TaskItem
            {
                Id = 2,
                Title = "Tenant A Task 2",
                Description = "Completed seeded task",
                IsCompleted = true,
                CreatedAtUtc = new DateTime(2026, 1, 2, 0, 0, 0, DateTimeKind.Utc),
                TenantId = 1,
                CreatedByUserId = 2
            },
            new TaskItem
            {
                Id = 3,
                Title = "Tenant B Task 1",
                Description = "Tenant B isolated task",
                IsCompleted = false,
                CreatedAtUtc = new DateTime(2026, 1, 3, 0, 0, 0, DateTimeKind.Utc),
                TenantId = 2,
                CreatedByUserId = 3
            }
        );
    }
}