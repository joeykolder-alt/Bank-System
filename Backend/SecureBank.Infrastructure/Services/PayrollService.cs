using Microsoft.EntityFrameworkCore;
using SecureBank.Application.DTOs;
using SecureBank.Application.Services;
using SecureBank.Domain.Entities;
using SecureBank.Domain.Enums;
using SecureBank.Domain.Queries;
using SecureBank.Infrastructure.Persistence;

namespace SecureBank.Infrastructure.Services;

public class PayrollService : IPayrollService
{
    private readonly ApplicationDbContext _db;

    public PayrollService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<PayrollProfileDto> AssignPayrollAsync(AssignPayrollRequest request, CancellationToken cancellationToken = default)
    {
        var account = await _db.BankAccounts.FindAsync(new object[] { request.AccountId }, cancellationToken);
        if (account == null) throw new KeyNotFoundException("Account not found.");
        if (account.Type != AccountType.Employee) throw new InvalidOperationException("Account must be of type Employee.");
        if (request.PayDayOfMonth < 1 || request.PayDayOfMonth > 28) throw new ArgumentException("PayDayOfMonth must be between 1 and 28.");

        var existing = await _db.EmployeePayrollProfiles.FirstOrDefaultAsync(p => p.BankAccountId == request.AccountId, cancellationToken);
        if (existing != null) throw new InvalidOperationException("Payroll already assigned for this account.");

        var nextRun = NextPayDate(DateTime.UtcNow, request.PayDayOfMonth);
        var profile = new EmployeePayrollProfile
        {
            BankAccountId = request.AccountId,
            MonthlySalary = request.MonthlySalary,
            PayDayOfMonth = request.PayDayOfMonth,
            NextRunAt = nextRun,
            IsActive = true
        };
        _db.EmployeePayrollProfiles.Add(profile);
        await _db.SaveChangesAsync(cancellationToken);

        return new PayrollProfileDto(profile.Id, profile.BankAccountId, account.IBAN, profile.MonthlySalary, profile.PayDayOfMonth, profile.NextRunAt, profile.IsActive);
    }

    public async Task<IReadOnlyList<PayrollProfileDto>> GetPayrollProfilesAsync(CancellationToken cancellationToken = default)
    {
        return await _db.EmployeePayrollProfiles
            .Include(p => p.BankAccount)
            .Select(p => new PayrollProfileDto(
                p.Id, 
                p.BankAccountId, 
                p.BankAccount != null ? p.BankAccount.IBAN : null, 
                p.MonthlySalary, 
                p.PayDayOfMonth, 
                p.NextRunAt, 
                p.IsActive))
            .ToListAsync(cancellationToken);
    }

    internal static DateTime NextPayDate(DateTime from, int payDay)
    {
        var d = new DateTime(from.Year, from.Month, Math.Min(payDay, DateTime.DaysInMonth(from.Year, from.Month)), 0, 0, 0, DateTimeKind.Utc);
        if (d <= from) d = d.AddMonths(1);
        return new DateTime(d.Year, d.Month, Math.Min(payDay, DateTime.DaysInMonth(d.Year, d.Month)), 0, 0, 0, DateTimeKind.Utc);
    }
}
