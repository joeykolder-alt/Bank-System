using System.ComponentModel.DataAnnotations.Schema;

namespace SecureBank.Domain.Entities;

/**
 *  NOTE:
 *    SenderBalanceBefore, ReceiverBalanceBefore, SenderBalanceAfter, ReceiverBalanceAfter
 *    are used for correctness and disaster recovery if account balances where messed up
 *
**/



[Table("transactions")]
public class Transaction
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    // Balance tracking for auditing and disaster recovery
    public decimal SenderBalanceBefore { get; set; }
    public decimal SenderBalanceAfter { get; set; }
    public decimal ReceiverBalanceBefore { get; set; }
    public decimal ReceiverBalanceAfter { get; set; }

    // Transfer details
    public decimal TransferAmount { get; set; }
    public decimal TransferFee { get; set; }
    public string Currency { get; set; } = "IQD";

    // Account references
    public Guid SenderId { get; set; }
    public Guid ReceiverId { get; set; }
    
    [ForeignKey(nameof(SenderId))]
    public BankAccount? Sender { get; set; }
    
    [ForeignKey(nameof(ReceiverId))]
    public BankAccount? Receiver { get; set; }

    // Payment link reference (if applicable)
    public Guid? PaymentLinkId { get; set; }
    
    [ForeignKey(nameof(PaymentLinkId))]
    public PaymentLink? PaymentLink { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
