using BankAccount.Domain.Exceptions;
using System.ComponentModel.DataAnnotations;

namespace BankAccount.Domain.Entities;

public class Account
{
    // Primary Key
    public long Id { get; private set; }

    // Domain Attributes
    public decimal Balance { get; private set; }
    public string AccountNumber { get; private set; }

    // Audit Fields
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public string CreatedBy { get; private set; }
    public DateTime? LastModifiedAt { get; private set; }
    public string? LastModifiedBy { get; private set; }

    // Concurrency Control
    [Timestamp]
    public byte[] RowVersion { get; private set; }

    public void Withdraw(decimal amount, string modifiedBy)
    {

        if (Balance < amount)
            throw new InsufficientFundsException(Balance, amount);

        Balance -= amount;
        UpdateAuditFields(modifiedBy);
    }

    // Audit Helper
    private void UpdateAuditFields(string modifiedBy)
    {
        LastModifiedAt = DateTime.UtcNow;
        LastModifiedBy = modifiedBy;
    }

}