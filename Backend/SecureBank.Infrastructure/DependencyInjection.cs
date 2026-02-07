using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SecureBank.Application.Common.Interfaces;
using SecureBank.Application.Common.Options;
using SecureBank.Application.Services;
using SecureBank.Infrastructure.Background;
using SecureBank.Infrastructure.Options;
using SecureBank.Infrastructure.Persistence;
using SecureBank.Infrastructure.Services;

namespace SecureBank.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<BankOptions>(configuration.GetSection(BankOptions.SectionName));
        services.Configure<JwtOptions>(configuration.GetSection("Jwt"));
        services.Configure<KycStorageOptions>(configuration.GetSection("KycStorage").Bind);

        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(connectionString, npgsql =>
                npgsql.MigrationsAssembly("SecureBank.Infrastructure")));

        services.AddScoped<IApplicationDbContext>(sp => sp.GetRequiredService<ApplicationDbContext>());
        services.AddIdentity<IdentityUser, IdentityRole>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequiredLength = 8;
        })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddScoped<IIbanGenerator, IbanGenerator>();
        services.AddScoped<IKycImageStorage, FileKycImageStorage>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IKycService, KycService>();
        services.AddScoped<IAdminKycService, AdminKycService>();
        services.AddScoped<IProfileService, ProfileService>();
        services.AddScoped<IAccountService, AccountService>();
        services.AddScoped<IMoneyService, MoneyService>();
        services.AddScoped<ITransactionService, TransactionService>();
        services.AddScoped<ITreasuryService, TreasuryService>();
        services.AddScoped<IPayrollService, PayrollService>();
        services.AddScoped<IPaymentLinkService, PaymentLinkService>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddHttpContextAccessor();

        services.AddHostedService<PayrollHostedService>();

        return services;
    }
}
