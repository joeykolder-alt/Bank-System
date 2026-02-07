using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SecureBank.Domain.Enums;
using SecureBank.Domain.Entities;
using SecureBank.Infrastructure.Persistence;
using SecureBank.Infrastructure.Services;
using SecureBank.Domain.Queries;

namespace SecureBank.Infrastructure.Background;

public class PayrollHostedService : BackgroundService
{
    private readonly IServiceProvider _services;
    private readonly ILogger<PayrollHostedService> _logger;

    public PayrollHostedService(IServiceProvider services, ILogger<PayrollHostedService> logger)
    {
        _services = services;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await RunPayrollAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Payroll run failed.");
            }

            await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
        }
    }

    private async Task RunPayrollAsync(CancellationToken cancellationToken)
    {
        using var scope = _services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var today = DateTime.UtcNow.Date;
        var dayOfMonth = today.Day;
        if (dayOfMonth > 28) dayOfMonth = 28;

        var profiles = await db.EmployeePayrollProfiles
            .Include(p => p.BankAccount)
            .Where(p => p.IsActive && p.PayDayOfMonth == dayOfMonth && p.NextRunAt <= DateTime.UtcNow)
            .ToListAsync(cancellationToken);

        foreach (var profile in profiles)
        {
            try
            {
                using var transactionScope = _services.CreateScope();
                var transactionDb = transactionScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var transactionQueries = new TransactionsQueries(transactionDb);

                await transactionQueries.Deposit(profile.BankAccount!.IBAN, profile.MonthlySalary, "IQD");

                profile.NextRunAt = PayrollService.NextPayDate(DateTime.UtcNow.AddMonths(1), profile.PayDayOfMonth);
            }
            catch (TransferException ex)
            {
                _logger.LogWarning("Payroll failed for account {AccountId}: {Error}", profile.BankAccountId, ex.ErrCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Payroll failed for account {AccountId}", profile.BankAccountId);
            }
        }

        var count = profiles.Count;
        if (count > 0)
        {
            await db.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Payroll processed {Count} employees.", count);
        }
    }
}
