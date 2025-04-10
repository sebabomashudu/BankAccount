namespace BankAccount.Domain.Exceptions;

public class InsufficientFundsException : Exception
{
    public decimal CurrentBalance { get; }
    public decimal RequestedAmount { get; }

    public InsufficientFundsException(decimal current, decimal requested)
        : base($"Insufficient funds. Current: {current}, Requested: {requested}")
    {
        CurrentBalance = current;
        RequestedAmount = requested;
    }
}