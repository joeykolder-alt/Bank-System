using SecureBank.Application.DTOs;

namespace SecureBank.Application.Services;

/// <summary>
/// Transaction Service - READ-ONLY access to the transaction ledger.
/// NEVER creates, updates, or deletes transactions.
/// Use TransactionQueries for creating ledger entries.
/// </summary>
public interface ITransactionService
{
    /// <summary>
    /// Gets transactions for a specific account (by account ID)
    /// </summary>
    Task<PagedResultDto<TransactionDto>> GetAccountTransactionsAsync(
        Guid accountId, 
        int page = 1, 
        int pageSize = 20, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets transactions for a specific account by IBAN
    /// </summary>
    Task<PagedResultDto<TransactionDto>> GetAccountTransactionsByIbanAsync(
        string iban, 
        int page = 1, 
        int pageSize = 20, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a single transaction by ID
    /// </summary>
    Task<TransactionDto?> GetTransactionByIdAsync(Guid transactionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all transactions for a specific payment link
    /// </summary>
    Task<PagedResultDto<TransactionDto>> GetPaymentLinkTransactionsAsync(
        Guid paymentLinkId, 
        string userId,
        int page = 1, 
        int pageSize = 20, 
        CancellationToken cancellationToken = default);
}
