namespace BankAccount.Domain.Entities;

public class AuditLog
{
    public Guid Id { get; private set; }
    public string Action { get; private set; } // "WITHDRAWAL", "DEPOSIT"
    public string EntityType { get; private set; } // "Account"
    public long EntityId { get; private set; }
    public string? OldValues { get; private set; } 
    public string? NewValues { get; private set; } 
    public DateTime Timestamp { get; private set; }
    public string? User { get; private set; }

    // Private constructor for EF Core
    private AuditLog() { }

    public AuditLog(
        string action,
        string entityType,
        long entityId,
        string? oldValues = null,
        string? newValues = null,
        string? user = null)
    {
        Id = Guid.NewGuid();
        Action = action;
        EntityType = entityType;
        EntityId = entityId;
        OldValues = oldValues;
        NewValues = newValues;
        User = user;
        Timestamp = DateTime.UtcNow;
    }
}