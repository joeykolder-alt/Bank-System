using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SecureBank.Application.Common.Interfaces;
using SecureBank.Application.DTOs;
using SecureBank.Application.Exceptions;
using SecureBank.Application.Services;
using SecureBank.Domain.Enums;

namespace SecureBank.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly IJwtTokenGenerator _jwt;
    private readonly IApplicationDbContext _db;

    public AuthService(
        UserManager<IdentityUser> userManager,
        SignInManager<IdentityUser> signInManager,
        IJwtTokenGenerator jwt,
        IApplicationDbContext db)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwt = jwt;
        _db = db;
    }

    public async Task<RegisterResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        var user = new IdentityUser
        {
            UserName = request.Email,
            Email = request.Email,
            EmailConfirmed = true
        };
        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            var isDuplicate = result.Errors.Any(e =>
                string.Equals(e.Code, "DuplicateUserName", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(e.Code, "DuplicateEmail", StringComparison.OrdinalIgnoreCase));
            if (isDuplicate)
                throw new ConflictException("Email already registered.");
            throw new InvalidOperationException(string.Join("; ", result.Errors.Select(e => e.Description)));
        }

        await _userManager.AddToRoleAsync(user, "User");
        return new RegisterResponse(Guid.Parse(user.Id), user.Email!);
    }

    public async Task<LoginResponse?> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null) return null;
        var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: false);
        if (!result.Succeeded) return null;

        var roles = await _userManager.GetRolesAsync(user);
        var role = roles.FirstOrDefault() ?? "User";

        // User role: allow login only if KYC is approved
        if (role == "User")
        {
            var hasApprovedKyc = await _db.KycApplications
                .AnyAsync(a => a.UserId == user.Id && a.Status == KycStatus.Approved, cancellationToken);
            if (!hasApprovedKyc)
                throw new ForbiddenException("Your account is awaiting KYC approval.");
        }

        var (token, expiresAt) = _jwt.Generate(user.Id, user.Email!, roles);
        return new LoginResponse(token, expiresAt, role);
    }
}
