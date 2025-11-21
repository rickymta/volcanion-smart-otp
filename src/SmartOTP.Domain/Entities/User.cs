using SmartOTP.Domain.Common;

namespace SmartOTP.Domain.Entities;

public class User : BaseEntity
{
    public string Email { get; private set; } = null!;
    public string PasswordHash { get; private set; } = null!;
    public string? FirstName { get; private set; }
    public string? LastName { get; private set; }
    public bool IsEmailVerified { get; private set; }
    public DateTime? LastLoginAt { get; private set; }
    public string? RefreshToken { get; private set; }
    public DateTime? RefreshTokenExpiryTime { get; private set; }

    // Navigation properties
    private readonly List<OtpAccount> _otpAccounts = [];
    public IReadOnlyCollection<OtpAccount> OtpAccounts => _otpAccounts.AsReadOnly();

    private readonly List<AuditLog> _auditLogs = [];
    public IReadOnlyCollection<AuditLog> AuditLogs => _auditLogs.AsReadOnly();

    private User() { } // EF Core constructor

    private User(string email, string passwordHash, string? firstName, string? lastName)
    {
        Email = email;
        PasswordHash = passwordHash;
        FirstName = firstName;
        LastName = lastName;
        IsEmailVerified = false;
    }

    public static User Create(string email, string passwordHash, string? firstName = null, string? lastName = null)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be empty", nameof(email));

        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentException("Password hash cannot be empty", nameof(passwordHash));

        var user = new User(email.ToLowerInvariant(), passwordHash, firstName, lastName);
        user.AddDomainEvent(new Events.UserRegisteredEvent(user.Id, user.Email));
        return user;
    }

    public void UpdateProfile(string? firstName, string? lastName)
    {
        FirstName = firstName;
        LastName = lastName;
        MarkAsUpdated();
    }

    public void VerifyEmail()
    {
        IsEmailVerified = true;
        MarkAsUpdated();
    }

    public void UpdatePassword(string newPasswordHash)
    {
        if (string.IsNullOrWhiteSpace(newPasswordHash))
            throw new ArgumentException("Password hash cannot be empty", nameof(newPasswordHash));

        PasswordHash = newPasswordHash;
        MarkAsUpdated();
    }

    public void SetRefreshToken(string refreshToken, DateTime expiryTime)
    {
        RefreshToken = refreshToken;
        RefreshTokenExpiryTime = expiryTime;
        MarkAsUpdated();
    }

    public void ClearRefreshToken()
    {
        RefreshToken = null;
        RefreshTokenExpiryTime = null;
        MarkAsUpdated();
    }

    public void RecordLogin()
    {
        LastLoginAt = DateTime.UtcNow;
        MarkAsUpdated();
        AddDomainEvent(new Events.UserLoggedInEvent(Id, Email));
    }

    public void AddOtpAccount(OtpAccount account)
    {
        _otpAccounts.Add(account);
        MarkAsUpdated();
    }

    public bool IsRefreshTokenValid()
    {
        return !string.IsNullOrEmpty(RefreshToken) 
               && RefreshTokenExpiryTime.HasValue 
               && RefreshTokenExpiryTime.Value > DateTime.UtcNow;
    }
}
