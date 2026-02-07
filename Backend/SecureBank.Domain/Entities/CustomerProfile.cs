using System.ComponentModel.DataAnnotations.Schema;
using SecureBank.Domain.Enums;

namespace SecureBank.Domain.Entities;

[Table("users")]
public class CustomerProfile : BaseEntity
{
    public required string UserId { get; set; }  // Auth system user ID
    public required string FullName { get; set; }
    public required string Email { get; set; }
    public required string Phone { get; set; }
    public string? NationalIdNumber { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? Province { get; set; }
    
    public CustomerProfileStatus Status { get; set; } = CustomerProfileStatus.Active;
    
    // Navigation properties
    public ICollection<BankAccount> BankAccounts { get; set; } = new List<BankAccount>();
}