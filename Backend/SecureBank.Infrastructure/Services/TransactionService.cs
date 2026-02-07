using Microsoft.EntityFrameworkCore;
using SecureBank.Application.DTOs;
using SecureBank.Application.Services;
using SecureBank.Domain.Entities;
using SecureBank.Infrastructure.Persistence;

namespace SecureBank.Infrastructure.Services;

/// <summary>
/// Transaction Service - READ-ONLY access to the transaction ledger.
/// NEVER creates, updates, or deletes transactions.
/// All writes should go through TransactionQueries.
/// </summary>
public class TransactionService : ITransactionService
{
    private readonly ApplicationDbContext _db;

    public TransactionService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<PagedResultDto<TransactionDto>> GetAccountTransactionsAsync(
        Guid accountId,
        int page = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var query = _db.Transactions
            .Include(t => t.Sender)
            .Include(t => t.Receiver)
            .Where(t => t.SenderId == accountId || t.ReceiverId == accountId)
            .OrderByDescending(t => t.CreatedAt);

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(t => new TransactionDto(
                t.Id,
                t.SenderBalanceBefore,
                t.SenderBalanceAfter,
                t.ReceiverBalanceBefore,
                t.ReceiverBalanceAfter,
                t.TransferAmount,
                t.TransferFee,
                t.SenderId,
                t.Sender != null ? t.Sender.IBAN : null,
                t.ReceiverId,
                t.Receiver != null ? t.Receiver.IBAN : null,
                t.PaymentLinkId,
                t.Currency,
                t.CreatedAt
            ))
            .ToListAsync(cancellationToken);

        return new PagedResultDto<TransactionDto>(items, total, page, pageSize);
    }

    public async Task<PagedResultDto<TransactionDto>> GetAccountTransactionsByIbanAsync(
        string iban,
        int page = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var query = _db.Transactions
            .Include(t => t.Sender)
            .Include(t => t.Receiver)
            .Where(t => (t.Sender != null && t.Sender.IBAN == iban) || 
                        (t.Receiver != null && t.Receiver.IBAN == iban))
            .OrderByDescending(t => t.CreatedAt);

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(t => new TransactionDto(
                t.Id,
                t.SenderBalanceBefore,
                t.SenderBalanceAfter,
                t.ReceiverBalanceBefore,
                t.ReceiverBalanceAfter,
                t.TransferAmount,
                t.TransferFee,
                t.SenderId,
                t.Sender != null ? t.Sender.IBAN : null,
                t.ReceiverId,
                t.Receiver != null ? t.Receiver.IBAN : null,
                t.PaymentLinkId,
                t.Currency,
                t.CreatedAt
            ))
            .ToListAsync(cancellationToken);

        return new PagedResultDto<TransactionDto>(items, total, page, pageSize);
    }

    public async Task<TransactionDto?> GetTransactionByIdAsync(Guid transactionId, CancellationToken cancellationToken = default)
    {
        var t = await _db.Transactions
            .Include(t => t.Sender)
            .Include(t => t.Receiver)
            .FirstOrDefaultAsync(t => t.Id == transactionId, cancellationToken);

        if (t == null) return null;

        return new TransactionDto(
            t.Id,
            t.SenderBalanceBefore,
            t.SenderBalanceAfter,
            t.ReceiverBalanceBefore,
            t.ReceiverBalanceAfter,
            t.TransferAmount,
            t.TransferFee,
            t.SenderId,
            t.Sender?.IBAN,
            t.ReceiverId,
            t.Receiver?.IBAN,
            t.PaymentLinkId,
            t.Currency,
            t.CreatedAt
        );
    }

    public async Task<PagedResultDto<TransactionDto>> GetPaymentLinkTransactionsAsync(
        Guid paymentLinkId,
        string userId,
        int page = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        // Verify user owns the payment link's merchant account
        var paymentLink = await _db.PaymentLinks
            .Include(p => p.Merchant)
            .FirstOrDefaultAsync(p => p.Id == paymentLinkId && !p.IsDeleted, cancellationToken);

        if (paymentLink == null || paymentLink.Merchant?.OwnerUserId != userId)
        {
            return new PagedResultDto<TransactionDto>(new List<TransactionDto>(), 0, page, pageSize);
        }

        var query = _db.Transactions
            .Include(t => t.Sender)
            .Include(t => t.Receiver)
            .Where(t => t.PaymentLinkId == paymentLinkId)
            .OrderByDescending(t => t.CreatedAt);

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(t => new TransactionDto(
                t.Id,
                t.SenderBalanceBefore,
                t.SenderBalanceAfter,
                t.ReceiverBalanceBefore,
                t.ReceiverBalanceAfter,
                t.TransferAmount,
                t.TransferFee,
                t.SenderId,
                t.Sender != null ? t.Sender.IBAN : null,
                t.ReceiverId,
                t.Receiver != null ? t.Receiver.IBAN : null,
                t.PaymentLinkId,
                t.Currency,
                t.CreatedAt
            ))
            .ToListAsync(cancellationToken);

        return new PagedResultDto<TransactionDto>(items, total, page, pageSize);
    }
}
