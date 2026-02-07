using SecureBank.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace SecureBank.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<CustomerProfile> CustomerProfiles { get; }
    DbSet<KycApplication> KycApplications { get; }
    DbSet<BankAccount> BankAccounts { get; }
    DbSet<PaymentLink> PaymentLinks { get; }
    DbSet<TreasuryAccount> TreasuryAccounts { get; }
    DbSet<Transaction> Transactions { get; }
    DbSet<EmployeePayrollProfile> EmployeePayrollProfiles { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
