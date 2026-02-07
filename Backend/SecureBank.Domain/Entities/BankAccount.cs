using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SecureBank.Domain.Enums;

namespace SecureBank.Domain.Entities;

[Table("bank_accounts")]
public class BankAccount : SoftDeleteEntity
{
    [RegularExpression(@"^[A-Z]{2}[0-9]{2}[A-Z0-9]{1,30}$", ErrorMessage = "Invalid IBAN format")]
    public required string IBAN { get; set; }

    public required string OwnerUserId { get; set; }
    
    [ForeignKey(nameof(OwnerUserId))]
    public CustomerProfile? OwnerUser { get; set; }

    public AccountStatus Status { get; set; } = AccountStatus.Pending;
    public AccountType Type { get; set; } = AccountType.Current;
    public decimal Balance { get; set; } = 0;
    public string BalanceCurrency { get; set; } = "IQD";
    public decimal TransferFee { get; set; } = 0;

    public decimal MinBalance { get; set; } = 0;
    public decimal MaxBalance { get; set; } = 1000000000;
    public decimal MaxTransfer { get; set; } = 10000000;
    public decimal MinTransfer { get; set; } = 1;

    // For double-spending prevention
    public Guid? LastTransferReceiverId { get; set; }
    public DateTime? LastTransferTime { get; set; }

    // Sub-account support
    public bool IsPrimary { get; set; } = true;
    public Guid? ParentAccountId { get; set; }
    
    [ForeignKey(nameof(ParentAccountId))]
    public BankAccount? ParentAccount { get; set; }
}
