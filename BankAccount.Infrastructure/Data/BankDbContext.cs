// BankAccount.Infrastructure/Data/BankDbContext.cs
using BankAccount.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Text.Json;

namespace BankAccount.Infrastructure.Data;

public class BankDbContext : DbContext
{
    public DbSet<Account> Accounts { get; set; }
    public DbSet<AuditLog> AuditLogs { get; set; }

    public BankDbContext(DbContextOptions<BankDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Account Configuration
        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(a => a.Id);
            entity.Property(a => a.Balance)
                  .HasColumnType("decimal(18,2)")
                  .IsRequired();

            // Optimistic concurrency control
            entity.Property(a => a.RowVersion)
                  .IsRowVersion();
        });

        // Audit Log Configuration
        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasKey(a => a.Id);

            entity.Property(a => a.Action)
                  .HasMaxLength(50)
                  .IsRequired();

            entity.Property(a => a.EntityType)
                  .HasMaxLength(100)
                  .IsRequired();

            entity.Property(a => a.EntityId)
                  .IsRequired();

            entity.Property(a => a.OldValues)
                  .HasColumnType("nvarchar(max)");

            entity.Property(a => a.NewValues)
                  .HasColumnType("nvarchar(max)");

            entity.Property(a => a.Timestamp)
                  .IsRequired()
                  .HasDefaultValueSql("GETUTCDATE()");

            entity.Property(a => a.User)
                  .HasMaxLength(255);

            // Index for faster querying
            entity.HasIndex(a => new { a.EntityType, a.EntityId });
            entity.HasIndex(a => a.Timestamp);
        });

        base.OnModelCreating(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        await AuditChangesAsync();
        return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    private async Task AuditChangesAsync()
    {
        var auditEntries = OnBeforeSaveChanges();
        if (auditEntries.Any())
        {
            await AuditLogs.AddRangeAsync(auditEntries);
        }
    }

    private List<AuditLog> OnBeforeSaveChanges()
    {
        ChangeTracker.DetectChanges();
        var auditEntries = new List<AuditLog>();
        var user = GetCurrentUser();

        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.Entity is AuditLog || entry.State is EntityState.Detached or EntityState.Unchanged)
                continue;

            var auditEntry = new AuditLog(
                action: entry.State.ToString(),
                entityType: entry.Metadata.ClrType.Name,
                entityId: GetEntityId(entry),
                user: user)
            {
               /* OldValues = entry.State == EntityState.Modified
                    ? JsonSerializer.Serialize(entry.OriginalValues.ToObject())
                    : null,
                NewValues = entry.State == EntityState.Deleted
                    ? null
                    : JsonSerializer.Serialize(entry.CurrentValues.ToObject())*/
            };

            auditEntries.Add(auditEntry);
        }

        return auditEntries;
    }

    private long GetEntityId(EntityEntry entry)
    {
        var primaryKey = entry.Metadata.FindPrimaryKey();
        var id = entry.Property(primaryKey.Properties[0].Name).CurrentValue;
        return Convert.ToInt64(id);
    }

    private string? GetCurrentUser()
    {
        return "System"; // Placeholder
    }
}