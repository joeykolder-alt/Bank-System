namespace SecureBank.Application.Common.Options;

public class BankOptions
{
    public const string SectionName = "Bank";
    public bool FundNewApprovedUserFromTreasury { get; set; } = true;
    public decimal InitialFundingAmount { get; set; } = 100m;
}
