namespace BankAccount.Infrastructure.Repositories;

using BankAccount.Domain.Entities;
using BankAccount.Domain.Exceptions;
using BankAccount.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

public interface IAccountRepository
{
    Task<Account> GetByIdAsync(long accountId);
    Task UpdateAsync(Account account);
}

public class AccountRepository : IAccountRepository
{
    private readonly BankDbContext _context;

    public AccountRepository(BankDbContext context) => _context = context;

    public async Task<Account> GetByIdAsync(long accountId)
        => await _context.Accounts.FirstOrDefaultAsync(a => a.Id == accountId)
            ?? throw new AccountNotFoundException(accountId);

    public async Task UpdateAsync(Account account)
    {
        _context.Accounts.Update(account);
        await _context.SaveChangesAsync();
    }
}