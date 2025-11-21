namespace SmartOTP.Domain.ValueObjects;

public class OtpCode : Common.ValueObject
{
    public string Code { get; private set; }
    public DateTime GeneratedAt { get; private set; }
    public int ValidityPeriodSeconds { get; private set; }

    private OtpCode(string code, DateTime generatedAt, int validityPeriodSeconds)
    {
        Code = code;
        GeneratedAt = generatedAt;
        ValidityPeriodSeconds = validityPeriodSeconds;
    }

    public static OtpCode Create(string code, int validityPeriodSeconds = 30)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("OTP code cannot be empty", nameof(code));

        if (!code.All(char.IsDigit))
            throw new ArgumentException("OTP code must contain only digits", nameof(code));

        if (code.Length != 6 && code.Length != 8)
            throw new ArgumentException("OTP code must be 6 or 8 digits", nameof(code));

        if (validityPeriodSeconds <= 0)
            throw new ArgumentException("Validity period must be positive", nameof(validityPeriodSeconds));

        return new OtpCode(code, DateTime.UtcNow, validityPeriodSeconds);
    }

    public bool IsValid()
    {
        var expirationTime = GeneratedAt.AddSeconds(ValidityPeriodSeconds);
        return DateTime.UtcNow <= expirationTime;
    }

    public int GetRemainingSeconds()
    {
        var expirationTime = GeneratedAt.AddSeconds(ValidityPeriodSeconds);
        var remaining = (expirationTime - DateTime.UtcNow).TotalSeconds;
        return Math.Max(0, (int)remaining);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Code;
        yield return GeneratedAt;
        yield return ValidityPeriodSeconds;
    }
}
