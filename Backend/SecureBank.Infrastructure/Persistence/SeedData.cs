using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SecureBank.Domain.Entities;

namespace SecureBank.Infrastructure.Persistence;

public static class SeedData
{
    public const string AdminEmail = "admin@bank.local";
    public static readonly Guid TreasuryId = Guid.Parse("11111111-1111-1111-1111-111111111111");

    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<ApplicationDbContext>>();

        foreach (var roleName in new[] { "Admin", "User" })
        {
            if (await roleManager.RoleExistsAsync(roleName)) continue;
            await roleManager.CreateAsync(new IdentityRole(roleName));
        }

        var adminUser = await userManager.FindByEmailAsync(AdminEmail);
        if (adminUser == null)
        {
            adminUser = new IdentityUser
            {
                UserName = AdminEmail,
                Email = AdminEmail,
                EmailConfirmed = true
            };
            var result = await userManager.CreateAsync(adminUser, "Admin@12345");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
                logger.LogInformation("Seeded admin user.");
            }
            else
                logger.LogWarning("Failed to seed admin: {Errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        if (!await db.TreasuryAccounts.AnyAsync())
        {
            db.TreasuryAccounts.Add(new TreasuryAccount
            {
                Id = TreasuryId,
                Balance = 1_000_000_000.00m,
                Currency = "USD",
                CreatedAt = DateTime.UtcNow
            });
            await db.SaveChangesAsync();
            logger.LogInformation("Seeded Treasury account.");
        }
    }
}
