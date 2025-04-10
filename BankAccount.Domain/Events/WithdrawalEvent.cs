namespace BankAccount.Domain.Events;

public record WithdrawalEvent(
    decimal Amount,
    long AccountId,
    string Status
);