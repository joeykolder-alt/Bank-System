using Microsoft.EntityFrameworkCore;
using SecureBank.Application.Common.Interfaces;
using SecureBank.Application.DTOs;
using SecureBank.Application.Services;
using SecureBank.Domain.Entities;
using SecureBank.Domain.Enums;
using SecureBank.Domain.Queries;
using SecureBank.Infrastructure.Persistence;

namespace SecureBank.Infrastructure.Services;

public class AccountService : IAccountService
{
    private readonly ApplicationDbContext _db;
    private readonly IIbanGenerator _ibanGenerator;

    public AccountService(ApplicationDbContext db, IIbanGenerator ibanGenerator)
    {
        _db = db;
        _ibanGenerator = ibanGenerator;
    }

    public async Task<IReadOnlyList<BankAccountDto>> GetUserAccountsAsync(string userId, CancellationToken cancellationToken = default)
    {
        return await _db.BankAccounts
            .Where(a => a.OwnerUserId == userId && !a.IsDeleted)
            .OrderBy(a => a.IsPrimary ? 0 : 1)
            .ThenBy(a => a.CreatedAt)
            .Select(a => new BankAccountDto(
                a.Id, 
                a.IBAN, 
                (int)a.Type, 
                a.Balance, 
                a.BalanceCurrency,
                a.IsPrimary, 
                a.ParentAccountId, 
                a.CreatedAt))
            .ToListAsync(cancellationToken);
    }

    public async Task<BankAccountDto?> GetAccountAsync(Guid accountId, string userId, CancellationToken cancellationToken = default)
    {
        var acc = await _db.BankAccounts
            .FirstOrDefaultAsync(a => a.Id == accountId && a.OwnerUserId == userId && !a.IsDeleted, cancellationToken);
        
        return acc == null 
            ? null 
            : new BankAccountDto(
                acc.Id, 
                acc.IBAN, 
                (int)acc.Type, 
                acc.Balance, 
                acc.BalanceCurrency,
                acc.IsPrimary, 
                acc.ParentAccountId, 
                acc.CreatedAt);
    }

    public async Task<BankAccountDto?> CreateSubAccountAsync(string userId, CreateSubAccountRequest request, CancellationToken cancellationToken = default)
    {
        var parent = await _db.BankAccounts
            .FirstOrDefaultAsync(a => a.Id == request.ParentAccountId 
                                      && a.OwnerUserId == userId 
                                      && a.IsPrimary 
                                      && !a.IsDeleted, cancellationToken);
        if (parent == null) return null;

        if (!Enum.TryParse<AccountType>(request.AccountType, ignoreCase: true, out var accountType))
            return null;

        var subAccount = new BankAccount
        {
            OwnerUserId = userId,
            IBAN = _ibanGenerator.Generate(),
            Type = accountType,
            Balance = 0,
            BalanceCurrency = parent.BalanceCurrency,
            TransferFee = parent.TransferFee,
            MinBalance = parent.MinBalance,
            MaxBalance = parent.MaxBalance,
            MaxTransfer = parent.MaxTransfer,
            MinTransfer = parent.MinTransfer,
            IsPrimary = false,
            ParentAccountId = parent.Id
        };
        
        _db.BankAccounts.Add(subAccount);
        await _db.SaveChangesAsync(cancellationToken);

        // Handle initial deposit using TransactionQueries
        if (request.InitialDeposit.HasValue && request.InitialDeposit.Value > 0)
        {
            var transactionQueries = new TransactionsQueries(_db);
            try
            {
                await transactionQueries.Deposit(subAccount.IBAN, request.InitialDeposit.Value, subAccount.BalanceCurrency);
            }
            catch (TransferException)
            {
                // If deposit fails, the account is still created but without initial balance
            }
        }

        return new BankAccountDto(
            subAccount.Id, 
            subAccount.IBAN, 
            (int)subAccount.Type, 
            subAccount.Balance, 
            subAccount.BalanceCurrency,
            subAccount.IsPrimary, 
            subAccount.ParentAccountId, 
            subAccount.CreatedAt);
    }
}
