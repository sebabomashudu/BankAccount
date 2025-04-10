
using BankAccount.Core.Interfaces;
using BankAccount.Domain.Entities;
using BankAccount.Domain.Events;
using BankAccount.Domain.Exceptions;
using BankAccount.Infrastructure.Repositories;
using Amazon.SimpleNotificationService;
using Polly;
using Microsoft.Extensions.Configuration;

namespace BankAccount.Core.Services;
public class AccountService : IAccountService
{
    private readonly IAccountRepository _repository;
    private readonly IAmazonSimpleNotificationService _sns;
    private readonly string _topicArn;
    private readonly IAsyncPolicy _retryPolicy;

    public AccountService(
        IAccountRepository repository,
        IAmazonSimpleNotificationService sns,
        IConfiguration config)
    {
        _repository = repository;
        _sns = sns;
        _topicArn = config["AWS:WithdrawalTopicArn"]!;

        _retryPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(3, attempt => TimeSpan.FromSeconds(attempt * 2));
    }

    public async Task ProcessWithdrawalAsync(long accountId, decimal amount, string modifiedBy)
    {
        var account = await _repository.GetByIdAsync(accountId);
        account.Withdraw(amount, modifiedBy);
        await _repository.UpdateAsync(account);
        await PublishEventAsync(accountId, amount);
    }

    private async Task PublishEventAsync(long accountId, decimal amount)
    {
        var message = new WithdrawalEvent(amount, accountId, "COMPLETED").ToString();

        await _retryPolicy.ExecuteAsync(async () =>
        {
            await _sns.PublishAsync(_topicArn, message);
        });
    }
}