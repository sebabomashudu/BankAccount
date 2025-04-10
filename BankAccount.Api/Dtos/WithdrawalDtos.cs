using System.ComponentModel.DataAnnotations;

namespace BankAccount.Api.Dtos;

public record WithdrawalRequest(
    long AccountId,
    [Range(0.01, double.MaxValue)] decimal Amount,
    string modifiedBy
);

public record WithdrawalResponse(
    string Status,
    string Message
);