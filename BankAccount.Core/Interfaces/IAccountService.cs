namespace BankAccount.Core.Interfaces;

public interface IAccountService
{
    Task ProcessWithdrawalAsync(long accountId, decimal amount,string modifiedBy);
}