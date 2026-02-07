namespace SecureBank.Application.DTOs;

/// <summary>
/// Transaction DTO - represents a ledger entry in the transaction history.
/// The Transaction entity is an immutable ledger - never update or delete.
/// </summary>
public record TransactionDto(
    Guid Id,
    decimal SenderBalanceBefore,
    decimal SenderBalanceAfter,
    decimal ReceiverBalanceBefore,
    decimal ReceiverBalanceAfter,
    decimal TransferAmount,
    decimal TransferFee,
    Guid SenderId,
    string? SenderIban,
    Guid ReceiverId,
    string? ReceiverIban,
    Guid? PaymentLinkId,
    string Currency,
    DateTime CreatedAt
);

public record PagedResultDto<T>(
    IReadOnlyList<T> Items,
    int TotalCount,
    int Page,
    int PageSize
)
{
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    public bool HasNextPage => Page < TotalPages;
    public bool HasPreviousPage => Page > 1;
}
