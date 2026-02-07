
namespace SecureBank.Domain.Entities;

using SecureBank.Domain.Enums;

public class KycApplication
{
    public Guid Id { get; set; }
    public string UserId { get; set; } = null!;
    public string FullName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public string NationalIdNumber { get; set; } = null!;
    public AccountType RequestedPrimaryAccountType { get; set; }
    public string? NationalIdImagePath { get; set; }
    public string? ResidenceCardImagePath { get; set; }
    public KycStatus Status { get; set; }
    public DateTime SubmittedAt { get; set; }
    public DateTime? ReviewedAt { get; set; }
    public string? ReviewedByAdminId { get; set; }
    public string? RejectionReason { get; set; }
}
