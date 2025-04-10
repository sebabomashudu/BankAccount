namespace BankAccount.Domain.Exceptions;

public class AccountNotFoundException : Exception
{
    public long AccountId { get; }

    public AccountNotFoundException(long accountId)
        : base($"Account not found: {accountId}")
    {
        AccountId = accountId;
    }
}