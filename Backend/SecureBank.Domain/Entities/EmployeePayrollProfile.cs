using System.ComponentModel.DataAnnotations.Schema;

namespace SecureBank.Domain.Entities;

[Table("employee_payroll_profiles")]
public class EmployeePayrollProfile
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid BankAccountId { get; set; }
    public decimal MonthlySalary { get; set; }
    public int PayDayOfMonth { get; set; }
    public DateTime NextRunAt { get; set; }
    public bool IsActive { get; set; } = true;

    [ForeignKey(nameof(BankAccountId))]
    public BankAccount? BankAccount { get; set; }
}
