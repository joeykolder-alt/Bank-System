using Microsoft.EntityFrameworkCore;
using SecureBank.Application.DTOs;
using SecureBank.Application.Services;
using SecureBank.Infrastructure.Persistence;

namespace SecureBank.Infrastructure.Services;

public class ProfileService : IProfileService
{
    private readonly ApplicationDbContext _db;

    public ProfileService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<CustomerProfileDto?> GetMyProfileAsync(string userId, CancellationToken cancellationToken = default)
    {
        var profile = await _db.CustomerProfiles
            .FirstOrDefaultAsync(p => p.UserId == userId, cancellationToken);
        
        if (profile == null) return null;

        var primaryAccount = await _db.BankAccounts
            .Where(a => a.OwnerUserId == userId && a.IsPrimary && !a.IsDeleted)
            .Select(a => new BankAccountSummaryDto(a.Id, a.IBAN, (int)a.Type, a.Balance, a.IsPrimary))
            .FirstOrDefaultAsync(cancellationToken);

        return new CustomerProfileDto(
            profile.Id,
            profile.UserId,
            profile.FullName,
            profile.Email,
            profile.Phone,
            profile.NationalIdNumber,
            profile.Status,
            profile.CreatedAt,
            primaryAccount
        );
    }
}
