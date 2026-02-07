namespace SecureBank.Domain.Entities;

public class TreasuryAccount
{
    public Guid Id { get; set; }
    public decimal Balance { get; set; }
    public string Currency { get; set; } = "USD";
    public DateTime CreatedAt { get; set; }
}
