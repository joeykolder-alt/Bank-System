using System.ComponentModel.DataAnnotations.Schema;

namespace SecureBank.Domain.Entities;

[Table("payment_links")]
public class PaymentLink : SoftDeleteEntity
{
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "IQD";

    public Guid MerchantId { get; set; }

    [ForeignKey(nameof(MerchantId))]
    public BankAccount? Merchant { get; set; }

    public string ProductName { get; set; } = string.Empty;
    public string? ProductDescription { get; set; }
    public string? ProductImageUrl { get; set; }
}
