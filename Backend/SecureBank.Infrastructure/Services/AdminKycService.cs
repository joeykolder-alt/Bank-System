using Microsoft.EntityFrameworkCore;
using SecureBank.Application.Common.Interfaces;
using SecureBank.Application.Common.Options;
using SecureBank.Application.DTOs;
using SecureBank.Application.Services;
using SecureBank.Domain.Entities;
using SecureBank.Domain.Enums;
using SecureBank.Domain.Queries;
using SecureBank.Infrastructure.Persistence;
using Microsoft.Extensions.Options;

namespace SecureBank.Infrastructure.Services;

public class AdminKycService : IAdminKycService
{
    private readonly ApplicationDbContext _db;
    private readonly IKycImageStorage _storage;
    private readonly BankOptions _bankOptions;
    private readonly IIbanGenerator _ibanGenerator;

    public AdminKycService(ApplicationDbContext db, IKycImageStorage storage, IOptions<BankOptions> bankOptions, IIbanGenerator ibanGenerator)
    {
        _db = db;
        _storage = storage;
        _bankOptions = bankOptions.Value;
        _ibanGenerator = ibanGenerator;
    }

    public async Task<PagedResultDto<KycApplicationListItemDto>> GetApplicationsAsync(string? status, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = _db.KycApplications.AsQueryable();
        if (!string.IsNullOrWhiteSpace(status))
        {
            var statusFilter = status.Trim();
            KycStatus? kycStatus = statusFilter.ToLowerInvariant() switch
            {
                "pending" => KycStatus.Pending,
                "approved" => KycStatus.Approved,
                "rejected" => KycStatus.Rejected,
                _ => int.TryParse(statusFilter, out var statusInt) ? (KycStatus?)statusInt : null
            };
            if (kycStatus.HasValue)
                query = query.Where(a => a.Status == kycStatus.Value);
        }

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(a => a.SubmittedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(a => new KycApplicationListItemDto(
                a.Id,
                a.FullName,
                a.Email,
                a.PhoneNumber,
                a.NationalIdNumber,
                (int)a.RequestedPrimaryAccountType,
                a.Status,
                a.SubmittedAt,
                a.ReviewedAt,
                a.RejectionReason
            ))
            .ToListAsync(cancellationToken);
        return new PagedResultDto<KycApplicationListItemDto>(items, total, page, pageSize);
    }

    public async Task<KycApplicationDetailDto?> GetApplicationByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var app = await _db.KycApplications.FindAsync(new object[] { id }, cancellationToken);
        if (app == null) return null;
        return new KycApplicationDetailDto(
            app.Id,
            app.UserId,
            app.FullName,
            app.Email,
            app.PhoneNumber,
            app.NationalIdNumber,
            (int)app.RequestedPrimaryAccountType,
            app.Status,
            app.SubmittedAt,
            app.ReviewedAt,
            app.ReviewedByAdminId,
            app.RejectionReason
        );
    }

    public async Task<(byte[] Content, string ContentType)?> GetNationalIdImageAsync(Guid applicationId, CancellationToken cancellationToken = default)
    {
        var app = await _db.KycApplications.FindAsync(new object[] { applicationId }, cancellationToken);
        if (app?.NationalIdImagePath == null) return null;
        return await _storage.GetAsync(app.NationalIdImagePath, cancellationToken);
    }

    public async Task<(byte[] Content, string ContentType)?> GetResidenceCardImageAsync(Guid applicationId, CancellationToken cancellationToken = default)
    {
        var app = await _db.KycApplications.FindAsync(new object[] { applicationId }, cancellationToken);
        if (app?.ResidenceCardImagePath == null) return null;
        return await _storage.GetAsync(app.ResidenceCardImagePath, cancellationToken);
    }

    public async Task ApproveAsync(Guid applicationId, string adminUserId, CancellationToken cancellationToken = default)
    {
        var app = await _db.KycApplications.FindAsync(new object[] { applicationId }, cancellationToken);
        if (app == null) throw new KeyNotFoundException("KYC application not found.");
        if (app.Status != KycStatus.Pending) throw new InvalidOperationException("Application is not pending.");

        app.Status = KycStatus.Approved;
        app.ReviewedAt = DateTime.UtcNow;
        app.ReviewedByAdminId = adminUserId;

        // Create or get customer profile
        var profile = await _db.CustomerProfiles.FirstOrDefaultAsync(p => p.UserId == app.UserId, cancellationToken);
        if (profile == null)
        {
            profile = new CustomerProfile
            {
                UserId = app.UserId,
                FullName = app.FullName,
                Email = app.Email,
                Phone = app.PhoneNumber,
                NationalIdNumber = app.NationalIdNumber,
                Status = CustomerProfileStatus.Active
            };
            _db.CustomerProfiles.Add(profile);
            await _db.SaveChangesAsync(cancellationToken);
        }

        // Create primary bank account
        var primaryAccount = new BankAccount
        {
            OwnerUserId = app.UserId,
            IBAN = _ibanGenerator.Generate(),
            Type = app.RequestedPrimaryAccountType,
            Status = AccountStatus.Active,
            Balance = 0,
            BalanceCurrency = "IQD",
            TransferFee = 0,
            MinBalance = 0,
            MaxBalance = 1000000000,
            MaxTransfer = 10000000,
            MinTransfer = 1,
            IsPrimary = true,
            ParentAccountId = null
        };
        _db.BankAccounts.Add(primaryAccount);
        await _db.SaveChangesAsync(cancellationToken);

        // Fund new user from treasury if configured
        if (_bankOptions.FundNewApprovedUserFromTreasury && _bankOptions.InitialFundingAmount > 0)
        {
            var treasury = await _db.TreasuryAccounts.OrderBy(x => x.Id).FirstOrDefaultAsync(cancellationToken);
            if (treasury != null && treasury.Balance >= _bankOptions.InitialFundingAmount)
            {
                treasury.Balance -= _bankOptions.InitialFundingAmount;
                await _db.SaveChangesAsync(cancellationToken);

                // Use TransactionQueries to deposit - maintains ledger integrity
                var transactionQueries = new TransactionsQueries(_db);
                try
                {
                    await transactionQueries.Deposit(primaryAccount.IBAN, _bankOptions.InitialFundingAmount, "IQD");
                }
                catch (TransferException)
                {
                    // Rollback treasury if deposit fails
                    treasury.Balance += _bankOptions.InitialFundingAmount;
                    await _db.SaveChangesAsync(cancellationToken);
                }
            }
        }
    }

    public async Task RejectAsync(Guid applicationId, string adminUserId, string reason, CancellationToken cancellationToken = default)
    {
        var app = await _db.KycApplications.FindAsync(new object[] { applicationId }, cancellationToken);
        if (app == null) throw new KeyNotFoundException("KYC application not found.");
        if (app.Status != KycStatus.Pending) throw new InvalidOperationException("Application is not pending.");
        app.Status = KycStatus.Rejected;
        app.ReviewedAt = DateTime.UtcNow;
        app.ReviewedByAdminId = adminUserId;
        app.RejectionReason = reason;
        await _db.SaveChangesAsync(cancellationToken);
    }
}
