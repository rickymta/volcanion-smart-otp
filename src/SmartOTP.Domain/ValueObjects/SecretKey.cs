namespace SmartOTP.Domain.ValueObjects;

public class SecretKey : Common.ValueObject
{
    public string EncryptedValue { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private SecretKey(string encryptedValue, DateTime createdAt)
    {
        EncryptedValue = encryptedValue;
        CreatedAt = createdAt;
    }

    public static SecretKey Create(string encryptedValue)
    {
        if (string.IsNullOrWhiteSpace(encryptedValue))
            throw new ArgumentException("Encrypted value cannot be empty", nameof(encryptedValue));

        return new SecretKey(encryptedValue, DateTime.UtcNow);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return EncryptedValue;
        yield return CreatedAt;
    }
}
