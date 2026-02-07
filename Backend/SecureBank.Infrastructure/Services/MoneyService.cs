using SecureBank.Application.DTOs;
using SecureBank.Application.Exceptions;
using SecureBank.Application.Services;
using SecureBank.Domain.Queries;
using SecureBank.Infrastructure.Persistence;
using System.Text.RegularExpressions;

namespace SecureBank.Infrastructure.Services;

/// <summary>
/// Money Service - handles deposits, withdrawals, and transfers.
/// Uses TransactionQueries for all transaction creation (ledger writes).
/// TransactionQueries handles all DB validation within proper transactions.
/// </summary>
public class MoneyService : IMoneyService
{
    private readonly ApplicationDbContext _db;
    private static readonly Regex IbanRegex = new(@"^[A-Z]{2}[0-9]{2}[A-Z0-9]{1,30}$", RegexOptions.Compiled);

    public MoneyService(ApplicationDbContext db)
    {
        _db = db;
    }

    private static bool IsValidIban(string? iban) => 
        !string.IsNullOrWhiteSpace(iban) && IbanRegex.IsMatch(iban.Trim().ToUpperInvariant());

    public async Task DepositAsync(string userId, DepositRequest request, CancellationToken cancellationToken = default)
    {
        if (request.Amount <= 0)
            throw new ArgumentException("Amount must be positive.");

        if (!IsValidIban(request.Iban))
            throw new ArgumentException("Invalid IBAN format.");

        var transactionQueries = new TransactionsQueries(_db);

        try
        {
            await transactionQueries.Deposit(request.Iban.Trim(), request.Amount, "IQD");
        }
        catch (TransferException ex)
        {
            throw MapTransferException(ex);
        }
    }

    public async Task WithdrawAsync(string userId, WithdrawRequest request, CancellationToken cancellationToken = default)
    {
        if (request.Amount <= 0)
            throw new ArgumentException("Amount must be positive.");

        if (!IsValidIban(request.Iban))
            throw new ArgumentException("Invalid IBAN format.");

        var transactionQueries = new TransactionsQueries(_db);

        try
        {
            await transactionQueries.Withdraw(request.Iban.Trim(), request.Amount, "IQD");
        }
        catch (TransferException ex)
        {
            throw MapTransferException(ex);
        }
    }

    public async Task TransferAsync(string userId, TransferRequest request, CancellationToken cancellationToken = default)
    {
        if (request.Amount <= 0)
            throw new ArgumentException("Amount must be positive.");

        var fromIban = (request.FromIban ?? string.Empty).Trim();
        var toIban = (request.ToIban ?? string.Empty).Trim();

        if (!IsValidIban(fromIban))
            throw new ArgumentException("Invalid source IBAN format.");

        if (!IsValidIban(toIban))
            throw new ArgumentException("Invalid destination IBAN format.");

        if (string.Equals(fromIban, toIban, StringComparison.OrdinalIgnoreCase))
            throw new ConflictException("Source and destination IBAN cannot be the same.");

        var transactionQueries = new TransactionsQueries(_db);

        try
        {
            await transactionQueries.Transfer(fromIban, toIban, request.Amount, "IQD");
        }
        catch (TransferException ex)
        {
            throw MapTransferException(ex);
        }
    }

    /// <summary>
    /// Maps TransferException from TransactionQueries to appropriate HTTP exceptions
    /// </summary>
    private static Exception MapTransferException(TransferException ex)
    {
        return ex.ErrCode switch
        {
            TransferErrCode.SenderUnauthorized => new ForbiddenException("Sender account not found or not authorized."),
            TransferErrCode.ReceiverUnauthorized => new ForbiddenException("Receiver account not found or not authorized."),
            TransferErrCode.SenderInsufficientFunds => new ConflictException("Insufficient funds."),
            TransferErrCode.DoubleSpendingDetected => new ConflictException("Please wait before making another transfer to this recipient."),
            TransferErrCode.MaxTransferExceeded => new ConflictException("Transfer amount exceeds your maximum transfer limit."),
            TransferErrCode.MinTransferNotReached => new ConflictException("Transfer amount is below your minimum transfer limit."),
            TransferErrCode.TransferAmountTooLow => new ConflictException("Transfer amount is too low."),
            TransferErrCode.ExceededReceiverMaxBalance => new ConflictException("Transfer would exceed receiver's maximum balance."),
            TransferErrCode.CurrencyConversionRequired => new ConflictException("Currency conversion is required. Sender and receiver must have the same currency."),
            TransferErrCode.PaymentLinkNotFound => new KeyNotFoundException("Payment link not found."),
            TransferErrCode.RetryAgain => new ConflictException("Transaction conflict detected. Please retry."),
            TransferErrCode.InvalidAmount => new ArgumentException("Invalid transfer amount."),
            _ => new InvalidOperationException($"Transfer failed: {ex.ErrCode}")
        };
    }
}
