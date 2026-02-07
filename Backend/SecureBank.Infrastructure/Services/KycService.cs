using Microsoft.EntityFrameworkCore;
using SecureBank.Application.Common.Interfaces;
using SecureBank.Application.DTOs;
using SecureBank.Application.Services;
using SecureBank.Domain.Entities;
using SecureBank.Domain.Enums;
using SecureBank.Infrastructure.Persistence;

namespace SecureBank.Infrastructure.Services;

public class KycService : IKycService
{
    private readonly ApplicationDbContext _db;
    private readonly IKycImageStorage _storage;

    public KycService(ApplicationDbContext db, IKycImageStorage storage)
    {
        _db = db;
        _storage = storage;
    }

    public async Task<KycApplicationCreateResult> SubmitApplicationAsync(string userId, string fullName, string email, string phoneNumber, string nationalIdNumber, int requestedPrimaryAccountType, Stream? nationalIdImage, Stream? residenceCardImage, string nationalIdFileName, string residenceCardFileName, CancellationToken cancellationToken = default)
    {
        var hasPending = await _db.KycApplications.AnyAsync(a => a.UserId == userId && a.Status == KycStatus.Pending, cancellationToken);
        if (hasPending)
            throw new InvalidOperationException("You already have a pending request.");

        string? nationalIdPath = null;
        string? residencePath = null;
        if (nationalIdImage != null && !string.IsNullOrEmpty(nationalIdFileName))
            nationalIdPath = await _storage.SaveAsync(nationalIdFileName, nationalIdImage, cancellationToken);
        if (residenceCardImage != null && !string.IsNullOrEmpty(residenceCardFileName))
            residencePath = await _storage.SaveAsync(residenceCardFileName, residenceCardImage, cancellationToken);

        var app = new KycApplication
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            FullName = fullName,
            Email = email,
            PhoneNumber = phoneNumber,
            NationalIdNumber = nationalIdNumber,
            RequestedPrimaryAccountType = (AccountType)requestedPrimaryAccountType,
            NationalIdImagePath = nationalIdPath,
            ResidenceCardImagePath = residencePath,
            Status = KycStatus.Pending,
            SubmittedAt = DateTime.UtcNow
        };
        _db.KycApplications.Add(app);
        await _db.SaveChangesAsync(cancellationToken);
        return new KycApplicationCreateResult(app.Id, app.Status);
    }

    public async Task<IReadOnlyList<KycApplicationListItemDto>> GetMyApplicationsAsync(string userId, CancellationToken cancellationToken = default)
    {
        var list = await _db.KycApplications
            .Where(a => a.UserId == userId)
            .OrderByDescending(a => a.SubmittedAt)
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
        return list;
    }
}
