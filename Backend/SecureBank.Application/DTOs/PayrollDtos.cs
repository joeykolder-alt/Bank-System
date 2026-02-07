namespace SecureBank.Application.DTOs;

public record AssignPayrollRequest(Guid AccountId, decimal MonthlySalary, int PayDayOfMonth);

public record PayrollProfileDto(
    Guid Id,
    Guid BankAccountId,
    string? Iban,
    decimal MonthlySalary,
    int PayDayOfMonth,
    DateTime NextRunAt,
    bool IsActive
);
