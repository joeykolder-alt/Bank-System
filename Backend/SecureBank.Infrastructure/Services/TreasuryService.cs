using Microsoft.EntityFrameworkCore;
using SecureBank.Application.DTOs;
using SecureBank.Application.Services;
using SecureBank.Domain.Queries;
using SecureBank.Infrastructure.Persistence;

namespace SecureBank.Infrastructure.Services;

public class TreasuryService : ITreasuryService
{
    private readonly ApplicationDbContext _db;

    public TreasuryService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<TreasuryBalanceDto> GetBalanceAsync(CancellationToken cancellationToken = default)
    {
        var t = await _db.TreasuryAccounts.OrderBy(x => x.Id).FirstAsync(cancellationToken);
        return new TreasuryBalanceDto(t.Balance, t.Currency);
    }

    public async Task FundAccountAsync(string toIban, decimal amount, string? note, string adminUserId, CancellationToken cancellationToken = default)
    {
        if (amount <= 0) throw new ArgumentException("Amount must be positive.");
        
        var treasury = await _db.TreasuryAccounts.OrderBy(x => x.Id).FirstAsync(cancellationToken);
        if (treasury.Balance < amount) throw new InvalidOperationException("Insufficient treasury balance.");

        var toAccount = await _db.BankAccounts.FirstOrDefaultAsync(a => a.IBAN == toIban && !a.IsDeleted, cancellationToken);
        if (toAccount == null) throw new KeyNotFoundException("Destination account not found.");

        // Use TransactionQueries for the deposit - this creates the proper ledger entry
        var transactionQueries = new TransactionsQueries(_db);
        
        try
        {
            // Deduct from treasury manually (treasury is not a regular bank account)
            treasury.Balance -= amount;
            await _db.SaveChangesAsync(cancellationToken);
            
            // Use deposit to add to the account
            await transactionQueries.Deposit(toIban, amount, treasury.Currency);
        }
        catch (TransferException ex)
        {
            // Rollback treasury deduction if deposit fails
            treasury.Balance += amount;
            await _db.SaveChangesAsync(cancellationToken);
            throw new InvalidOperationException($"Failed to fund account: {ex.ErrCode}");
        }
    }
}
