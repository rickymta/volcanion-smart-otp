using SmartOTP.Domain.Common;
using SmartOTP.Domain.Enums;
using SmartOTP.Domain.ValueObjects;

namespace SmartOTP.Domain.Entities;

public class OtpAccount : BaseEntity
{
    public Guid UserId { get; private set; }
    public string Issuer { get; private set; } = null!;
    public string AccountName { get; private set; } = null!;
    public OtpType Type { get; private set; }
    public OtpAlgorithm Algorithm { get; private set; }
    public int Digits { get; private set; }
    public int Period { get; private set; } // For TOTP (in seconds)
    public long Counter { get; private set; } // For HOTP
    public SecretKey Secret { get; private set; } = null!;
    public string? IconUrl { get; private set; }
    public int SortOrder { get; private set; }

    // Navigation properties
    public User User { get; private set; } = null!;

    private OtpAccount() { } // EF Core constructor

    private OtpAccount(
        Guid userId,
        string issuer,
        string accountName,
        OtpType type,
        SecretKey secret,
        OtpAlgorithm algorithm = OtpAlgorithm.SHA1,
        int digits = 6,
        int period = 30,
        long counter = 0,
        string? iconUrl = null)
    {
        UserId = userId;
        Issuer = issuer;
        AccountName = accountName;
        Type = type;
        Secret = secret;
        Algorithm = algorithm;
        Digits = digits;
        Period = period;
        Counter = counter;
        IconUrl = iconUrl;
        SortOrder = 0;
    }

    public static OtpAccount CreateTOTP(
        Guid userId,
        string issuer,
        string accountName,
        SecretKey secret,
        OtpAlgorithm algorithm = OtpAlgorithm.SHA1,
        int digits = 6,
        int period = 30,
        string? iconUrl = null)
    {
        ValidateCommonParameters(issuer, accountName, digits);

        if (period <= 0)
            throw new ArgumentException("Period must be positive", nameof(period));

        var account = new OtpAccount(userId, issuer, accountName, OtpType.TOTP, secret, algorithm, digits, period, 0, iconUrl);
        account.AddDomainEvent(new Events.OtpAccountCreatedEvent(account.Id, account.UserId, account.Issuer, account.AccountName));
        return account;
    }

    public static OtpAccount CreateHOTP(
        Guid userId,
        string issuer,
        string accountName,
        SecretKey secret,
        OtpAlgorithm algorithm = OtpAlgorithm.SHA1,
        int digits = 6,
        long initialCounter = 0,
        string? iconUrl = null)
    {
        ValidateCommonParameters(issuer, accountName, digits);

        if (initialCounter < 0)
            throw new ArgumentException("Counter cannot be negative", nameof(initialCounter));

        var account = new OtpAccount(userId, issuer, accountName, OtpType.HOTP, secret, algorithm, digits, 0, initialCounter, iconUrl);
        account.AddDomainEvent(new Events.OtpAccountCreatedEvent(account.Id, account.UserId, account.Issuer, account.AccountName));
        return account;
    }

    private static void ValidateCommonParameters(string issuer, string accountName, int digits)
    {
        if (string.IsNullOrWhiteSpace(issuer))
            throw new ArgumentException("Issuer cannot be empty", nameof(issuer));

        if (string.IsNullOrWhiteSpace(accountName))
            throw new ArgumentException("Account name cannot be empty", nameof(accountName));

        if (digits != 6 && digits != 8)
            throw new ArgumentException("Digits must be 6 or 8", nameof(digits));
    }

    public void UpdateDetails(string issuer, string accountName, string? iconUrl = null)
    {
        if (string.IsNullOrWhiteSpace(issuer))
            throw new ArgumentException("Issuer cannot be empty", nameof(issuer));

        if (string.IsNullOrWhiteSpace(accountName))
            throw new ArgumentException("Account name cannot be empty", nameof(accountName));

        Issuer = issuer;
        AccountName = accountName;
        IconUrl = iconUrl;
        MarkAsUpdated();
        AddDomainEvent(new Events.OtpAccountUpdatedEvent(Id, UserId));
    }

    public void IncrementCounter()
    {
        if (Type != OtpType.HOTP)
            throw new InvalidOperationException("Cannot increment counter for TOTP accounts");

        Counter++;
        MarkAsUpdated();
    }

    public void UpdateSortOrder(int sortOrder)
    {
        if (sortOrder < 0)
            throw new ArgumentException("Sort order cannot be negative", nameof(sortOrder));

        SortOrder = sortOrder;
        MarkAsUpdated();
    }

    public void Delete()
    {
        MarkAsDeleted();
        AddDomainEvent(new Events.OtpAccountDeletedEvent(Id, UserId));
    }
}
