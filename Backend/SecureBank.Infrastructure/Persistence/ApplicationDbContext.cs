using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SecureBank.Application.Common.Interfaces;
using SecureBank.Domain.Entities;
using SecureBank.Domain.Enums;

namespace SecureBank.Infrastructure.Persistence;

public class ApplicationDbContext : IdentityDbContext, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<CustomerProfile> CustomerProfiles => Set<CustomerProfile>();
    public DbSet<KycApplication> KycApplications => Set<KycApplication>();
    public DbSet<BankAccount> BankAccounts => Set<BankAccount>();
    public DbSet<TreasuryAccount> TreasuryAccounts => Set<TreasuryAccount>();
    public DbSet<Transaction> Transactions => Set<Transaction>();
    public DbSet<EmployeePayrollProfile> EmployeePayrollProfiles => Set<EmployeePayrollProfile>();
    public DbSet<PaymentLink> PaymentLinks => Set<PaymentLink>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<CustomerProfile>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.UserId).HasColumnName("user_id");
            e.Property(x => x.FullName).HasColumnName("full_name");
            e.Property(x => x.Email).HasColumnName("email");
            e.Property(x => x.Phone).HasColumnName("phone");
            e.Property(x => x.NationalIdNumber).HasColumnName("national_id_number");
            e.Property(x => x.Address).HasColumnName("address");
            e.Property(x => x.City).HasColumnName("city");
            e.Property(x => x.Province).HasColumnName("province");
            e.Property(x => x.Status).HasColumnName("status").HasConversion<int>();
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.Property(x => x.UpdatedAt).HasColumnName("updated_at");
            e.HasIndex(x => x.UserId).IsUnique();
        });

        builder.Entity<KycApplication>(e =>
        {
            e.ToTable("kyc_applications");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.UserId).HasColumnName("user_id");
            e.Property(x => x.FullName).HasColumnName("full_name");
            e.Property(x => x.Email).HasColumnName("email");
            e.Property(x => x.PhoneNumber).HasColumnName("phone_number");
            e.Property(x => x.NationalIdNumber).HasColumnName("national_id_number");
            e.Property(x => x.RequestedPrimaryAccountType).HasColumnName("requested_primary_account_type").HasConversion<int>();
            e.Property(x => x.NationalIdImagePath).HasColumnName("national_id_image_path");
            e.Property(x => x.ResidenceCardImagePath).HasColumnName("residence_card_image_path");
            e.Property(x => x.Status).HasColumnName("status").HasConversion<int>();
            e.Property(x => x.SubmittedAt).HasColumnName("submitted_at");
            e.Property(x => x.ReviewedAt).HasColumnName("reviewed_at");
            e.Property(x => x.ReviewedByAdminId).HasColumnName("reviewed_by_admin_id");
            e.Property(x => x.RejectionReason).HasColumnName("rejection_reason");
            e.HasIndex(x => x.UserId).HasFilter("status = 0").IsUnique(); // Pending = 0
        });

        builder.Entity<BankAccount>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.IBAN).HasColumnName("iban");
            e.Property(x => x.OwnerUserId).HasColumnName("owner_user_id");
            e.Property(x => x.Status).HasColumnName("status").HasConversion<int>();
            e.Property(x => x.Type).HasColumnName("type").HasConversion<int>();
            e.Property(x => x.Balance).HasColumnName("balance").HasPrecision(18, 2);
            e.Property(x => x.BalanceCurrency).HasColumnName("balance_currency");
            e.Property(x => x.TransferFee).HasColumnName("transfer_fee").HasPrecision(18, 2);
            e.Property(x => x.MinBalance).HasColumnName("min_balance").HasPrecision(18, 2);
            e.Property(x => x.MaxBalance).HasColumnName("max_balance").HasPrecision(18, 2);
            e.Property(x => x.MaxTransfer).HasColumnName("max_transfer").HasPrecision(18, 2);
            e.Property(x => x.MinTransfer).HasColumnName("min_transfer").HasPrecision(18, 2);
            e.Property(x => x.LastTransferReceiverId).HasColumnName("last_transfer_receiver_id");
            e.Property(x => x.LastTransferTime).HasColumnName("last_transfer_time");
            e.Property(x => x.IsPrimary).HasColumnName("is_primary");
            e.Property(x => x.ParentAccountId).HasColumnName("parent_account_id");
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.Property(x => x.UpdatedAt).HasColumnName("updated_at");
            e.Property(x => x.IsDeleted).HasColumnName("is_deleted");
            e.Property(x => x.DeletedAt).HasColumnName("deleted_at");
            e.HasIndex(x => x.IBAN).IsUnique();
            e.HasOne(x => x.OwnerUser).WithMany(u => u.BankAccounts).HasForeignKey(x => x.OwnerUserId).HasPrincipalKey(u => u.UserId);
            e.HasOne(x => x.ParentAccount).WithMany().HasForeignKey(x => x.ParentAccountId);
        });


        builder.Entity<TreasuryAccount>(e =>
        {
            e.ToTable("treasury_accounts");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.Balance).HasColumnName("balance").HasPrecision(18, 2);
            e.Property(x => x.Currency).HasColumnName("currency");
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
        });

        builder.Entity<Transaction>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.SenderBalanceBefore).HasColumnName("sender_balance_before").HasPrecision(18, 2);
            e.Property(x => x.ReceiverBalanceBefore).HasColumnName("receiver_balance_before").HasPrecision(18, 2);
            e.Property(x => x.SenderBalanceAfter).HasColumnName("sender_balance_after").HasPrecision(18, 2);
            e.Property(x => x.ReceiverBalanceAfter).HasColumnName("receiver_balance_after").HasPrecision(18, 2);
            e.Property(x => x.TransferAmount).HasColumnName("transfer_amount").HasPrecision(18, 2);
            e.Property(x => x.TransferFee).HasColumnName("transfer_fee").HasPrecision(18, 2);
            e.Property(x => x.SenderId).HasColumnName("sender_id");
            e.Property(x => x.ReceiverId).HasColumnName("receiver_id");
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.Property(x => x.PaymentLinkId).HasColumnName("payment_link_id");
            e.Property(x => x.Currency).HasColumnName("currency");
            e.HasOne(x => x.Sender).WithMany().HasForeignKey(x => x.SenderId);
            e.HasOne(x => x.Receiver).WithMany().HasForeignKey(x => x.ReceiverId);
            e.HasOne(x => x.PaymentLink).WithMany().HasForeignKey(x => x.PaymentLinkId);
        });

        builder.Entity<EmployeePayrollProfile>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.BankAccountId).HasColumnName("bank_account_id");
            e.Property(x => x.MonthlySalary).HasColumnName("monthly_salary").HasPrecision(18, 2);
            e.Property(x => x.PayDayOfMonth).HasColumnName("pay_day_of_month");
            e.Property(x => x.NextRunAt).HasColumnName("next_run_at");
            e.Property(x => x.IsActive).HasColumnName("is_active");
            e.HasOne(x => x.BankAccount).WithMany().HasForeignKey(x => x.BankAccountId);
        });

        builder.Entity<PaymentLink>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.Amount).HasColumnName("amount").HasPrecision(18, 2);
            e.Property(x => x.Currency).HasColumnName("currency");
            e.Property(x => x.MerchantId).HasColumnName("merchant_id");
            e.Property(x => x.ProductName).HasColumnName("product_name");
            e.Property(x => x.ProductDescription).HasColumnName("product_description");
            e.Property(x => x.ProductImageUrl).HasColumnName("product_image_url");
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.Property(x => x.UpdatedAt).HasColumnName("updated_at");
            e.Property(x => x.IsDeleted).HasColumnName("is_deleted");
            e.Property(x => x.DeletedAt).HasColumnName("deleted_at");
            e.HasOne(x => x.Merchant).WithMany().HasForeignKey(x => x.MerchantId);
        });

        // Identity tables: aspnetusers, aspnetroles, etc.
        builder.Entity<Microsoft.AspNetCore.Identity.IdentityUser>(e => e.ToTable("aspnetusers"));
        builder.Entity<Microsoft.AspNetCore.Identity.IdentityRole>(e => e.ToTable("aspnetroles"));
        builder.Entity<Microsoft.AspNetCore.Identity.IdentityUserRole<string>>(e => e.ToTable("aspnetuserroles"));
        builder.Entity<Microsoft.AspNetCore.Identity.IdentityUserClaim<string>>(e => e.ToTable("aspnetuserclaims"));
        builder.Entity<Microsoft.AspNetCore.Identity.IdentityUserLogin<string>>(e => e.ToTable("aspnetuserlogins"));
        builder.Entity<Microsoft.AspNetCore.Identity.IdentityUserToken<string>>(e => e.ToTable("aspnetusertokens"));
        builder.Entity<Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>>(e => e.ToTable("aspnetroleclaims"));
    }
}
