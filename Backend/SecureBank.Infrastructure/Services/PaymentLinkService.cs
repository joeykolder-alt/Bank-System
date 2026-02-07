using Microsoft.EntityFrameworkCore;
using SecureBank.Application.DTOs;
using SecureBank.Application.Exceptions;
using SecureBank.Application.Services;
using SecureBank.Domain.Entities;
using SecureBank.Domain.Queries;
using SecureBank.Infrastructure.Persistence;
using System.Text.RegularExpressions;

namespace SecureBank.Infrastructure.Services;

/// <summary>
/// Payment Link Service - manages payment links for merchants.
/// Uses TransactionQueries for payment processing (ledger writes).
/// TransactionQueries handles all DB validation within proper transactions.
/// </summary>
public class PaymentLinkService : IPaymentLinkService
{
    private readonly ApplicationDbContext _db;
    private static readonly Regex IbanRegex = new(@"^[A-Z]{2}[0-9]{2}[A-Z0-9]{1,30}$", RegexOptions.Compiled);

    public PaymentLinkService(ApplicationDbContext db)
    {
        _db = db;
    }

    private static bool IsValidIban(string? iban) => 
        !string.IsNullOrWhiteSpace(iban) && IbanRegex.IsMatch(iban.Trim().ToUpperInvariant());

    public async Task<PaymentLinkDto> CreatePaymentLinkAsync(
        string userId, CreatePaymentLinkRequest request, CancellationToken cancellationToken = default)
    {
        if (request.Amount <= 0)
            throw new ArgumentException("Amount must be positive.");

        if (string.IsNullOrWhiteSpace(request.ProductName))
            throw new ArgumentException("Product name is required.");

        if (!IsValidIban(request.MerchantIban))
            throw new ArgumentException("Invalid merchant IBAN format.");

        // Find merchant account - needed to get MerchantId for the payment link
        var merchantAccount = await _db.BankAccounts
            .FirstOrDefaultAsync(a => a.IBAN == request.MerchantIban.Trim() 
                                      && a.OwnerUserId == userId 
                                      && !a.IsDeleted, cancellationToken);

        if (merchantAccount == null)
            throw new ForbiddenException("Merchant account not found or you don't own it.");

        var paymentLink = new PaymentLink
        {
            Amount = request.Amount,
            Currency = request.Currency,
            MerchantId = merchantAccount.Id,
            ProductName = request.ProductName,
            ProductDescription = request.ProductDescription,
            ProductImageUrl = request.ProductImageUrl
        };

        _db.PaymentLinks.Add(paymentLink);
        await _db.SaveChangesAsync(cancellationToken);

        return new PaymentLinkDto(
            paymentLink.Id,
            paymentLink.Amount,
            paymentLink.Currency,
            paymentLink.MerchantId,
            merchantAccount.IBAN,
            paymentLink.ProductName,
            paymentLink.ProductDescription,
            paymentLink.ProductImageUrl,
            paymentLink.CreatedAt
        );
    }

    public async Task<PaymentLinkDto> GetPaymentLinkAsync(Guid paymentLinkId, CancellationToken cancellationToken = default)
    {
        var paymentLink = await _db.PaymentLinks
            .Include(p => p.Merchant)
            .FirstOrDefaultAsync(p => p.Id == paymentLinkId && !p.IsDeleted, cancellationToken);

        if (paymentLink == null)
            throw new KeyNotFoundException("Payment link not found.");

        return new PaymentLinkDto(
            paymentLink.Id,
            paymentLink.Amount,
            paymentLink.Currency,
            paymentLink.MerchantId,
            paymentLink.Merchant?.IBAN,
            paymentLink.ProductName,
            paymentLink.ProductDescription,
            paymentLink.ProductImageUrl,
            paymentLink.CreatedAt
        );
    }

    public async Task<IReadOnlyList<PaymentLinkDto>> GetMerchantPaymentLinksAsync(
        string userId, CancellationToken cancellationToken = default)
    {
        // Get all merchant accounts owned by user
        var merchantAccountIds = await _db.BankAccounts
            .Where(a => a.OwnerUserId == userId && !a.IsDeleted)
            .Select(a => a.Id)
            .ToListAsync(cancellationToken);

        var paymentLinks = await _db.PaymentLinks
            .Include(p => p.Merchant)
            .Where(p => merchantAccountIds.Contains(p.MerchantId) && !p.IsDeleted)
            .OrderByDescending(p => p.CreatedAt)
            .Select(p => new PaymentLinkDto(
                p.Id,
                p.Amount,
                p.Currency,
                p.MerchantId,
                p.Merchant != null ? p.Merchant.IBAN : null,
                p.ProductName,
                p.ProductDescription,
                p.ProductImageUrl,
                p.CreatedAt
            ))
            .ToListAsync(cancellationToken);

        return paymentLinks;
    }

    public async Task<PaymentLinkDto> UpdatePaymentLinkAsync(
        string userId, Guid paymentLinkId, UpdatePaymentLinkRequest request, CancellationToken cancellationToken = default)
    {
        var paymentLink = await _db.PaymentLinks
            .Include(p => p.Merchant)
            .FirstOrDefaultAsync(p => p.Id == paymentLinkId && !p.IsDeleted, cancellationToken);

        if (paymentLink == null)
            throw new KeyNotFoundException("Payment link not found.");

        // Verify ownership
        if (paymentLink.Merchant?.OwnerUserId != userId)
            throw new ForbiddenException("You don't own this payment link.");

        // Update only provided fields
        if (request.Amount.HasValue)
        {
            if (request.Amount.Value <= 0)
                throw new ArgumentException("Amount must be positive.");
            paymentLink.Amount = request.Amount.Value;
        }

        if (!string.IsNullOrWhiteSpace(request.Currency))
            paymentLink.Currency = request.Currency;

        if (!string.IsNullOrWhiteSpace(request.ProductName))
            paymentLink.ProductName = request.ProductName;

        if (request.ProductDescription != null)
            paymentLink.ProductDescription = request.ProductDescription;

        if (request.ProductImageUrl != null)
            paymentLink.ProductImageUrl = request.ProductImageUrl;

        paymentLink.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync(cancellationToken);

        return new PaymentLinkDto(
            paymentLink.Id,
            paymentLink.Amount,
            paymentLink.Currency,
            paymentLink.MerchantId,
            paymentLink.Merchant?.IBAN,
            paymentLink.ProductName,
            paymentLink.ProductDescription,
            paymentLink.ProductImageUrl,
            paymentLink.CreatedAt
        );
    }

    public async Task DeletePaymentLinkAsync(
        string userId, Guid paymentLinkId, CancellationToken cancellationToken = default)
    {
        var paymentLink = await _db.PaymentLinks
            .Include(p => p.Merchant)
            .FirstOrDefaultAsync(p => p.Id == paymentLinkId && !p.IsDeleted, cancellationToken);

        if (paymentLink == null)
            throw new KeyNotFoundException("Payment link not found.");

        // Verify ownership
        if (paymentLink.Merchant?.OwnerUserId != userId)
            throw new ForbiddenException("You don't own this payment link.");

        // Soft delete - NEVER hard delete as transactions may reference this
        paymentLink.IsDeleted = true;
        paymentLink.DeletedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task PayPaymentLinkAsync(
        string userId, PayPaymentLinkRequest request, CancellationToken cancellationToken = default)
    {
        // Only validate IBAN syntax - TransactionQueries handles all DB checks
        if (!IsValidIban(request.SenderIban))
            throw new ArgumentException("Invalid sender IBAN format.");

        // Get payment link to know the merchant ID
        var paymentLink = await _db.PaymentLinks
            .Include(p => p.Merchant)
            .FirstOrDefaultAsync(p => p.Id == request.PaymentLinkId && !p.IsDeleted, cancellationToken);

        if (paymentLink == null)
            throw new KeyNotFoundException("Payment link not found.");

        if (paymentLink.Merchant == null)
            throw new InvalidOperationException("Payment link has no merchant account.");

        // Use TransactionQueries - it handles all validation within a proper transaction
        var transactionQueries = new TransactionsQueries(_db);

        try
        {
            await transactionQueries.PayPaymentLink(
                request.SenderIban.Trim(),
                paymentLink.Merchant.IBAN,
                paymentLink.Amount,
                request.PaymentLinkId,
                paymentLink.Currency
            );
        }
        catch (TransferException ex)
        {
            throw MapTransferException(ex);
        }
    }

    private static Exception MapTransferException(TransferException ex)
    {
        return ex.ErrCode switch
        {
            TransferErrCode.SenderUnauthorized => new ForbiddenException("Sender account not found or not authorized."),
            TransferErrCode.ReceiverUnauthorized => new ForbiddenException("Merchant account not found or not authorized."),
            TransferErrCode.SenderInsufficientFunds => new ConflictException("Insufficient funds."),
            TransferErrCode.DoubleSpendingDetected => new ConflictException("Please wait before making another transfer."),
            TransferErrCode.MaxTransferExceeded => new ConflictException("Transfer amount exceeds your limit."),
            TransferErrCode.TransferAmountTooLow => new ConflictException("Transfer amount is below minimum."),
            TransferErrCode.ExceededReceiverMaxBalance => new ConflictException("Transfer would exceed merchant's maximum balance."),
            TransferErrCode.CurrencyConversionRequired => new ConflictException("Currency conversion is required for this transfer."),
            TransferErrCode.PaymentLinkNotFound => new KeyNotFoundException("Payment link not found."),
            TransferErrCode.RetryAgain => new ConflictException("Please retry the transaction."),
            TransferErrCode.InvalidAmount => new ArgumentException("Invalid payment amount."),
            _ => new InvalidOperationException($"Transfer failed: {ex.ErrCode}")
        };
    }
}
