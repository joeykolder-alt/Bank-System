using System.ComponentModel.DataAnnotations;

namespace SecureBank.Api.Models;

/// <summary>
/// Request model for POST /api/Auth/register-with-kyc (multipart/form-data).
/// Strongly-typed binding allows Swagger to show all form fields and file inputs,
/// and ensures validation runs before the action executes.
/// </summary>
public class RegisterWithKycRequest
{
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Enter a valid email address.")]
    [StringLength(256)]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required.")]
    [MinLength(8, ErrorMessage = "Password must be at least 8 characters.")]
    [StringLength(100)]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Full name is required.")]
    [StringLength(200)]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Phone number is required.")]
    [StringLength(50)]
    public string PhoneNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "National ID number is required.")]
    [StringLength(100)]
    public string NationalIdNumber { get; set; } = string.Empty;

    /// <summary>0 = Savings, 1 = Current, 2 = Business, 3 = Employee</summary>
    [Required(ErrorMessage = "Account type is required.")]
    [Range(0, 3, ErrorMessage = "Account type must be 0 (Savings), 1 (Current), 2 (Business), or 3 (Employee).")]
    public int RequestedPrimaryAccountType { get; set; }

    [Required(ErrorMessage = "National ID image is required.")]
    public IFormFile? NationalIdImage { get; set; }

    [Required(ErrorMessage = "Residence card image is required.")]
    public IFormFile? ResidenceCardImage { get; set; }
}
