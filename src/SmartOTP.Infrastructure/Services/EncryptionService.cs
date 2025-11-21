using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using SmartOTP.Application.Common.Interfaces;

namespace SmartOTP.Infrastructure.Services;

public class EncryptionService : IEncryptionService
{
    private readonly byte[] _key;
    private readonly byte[] _iv;

    public EncryptionService(IConfiguration configuration)
    {
        var encryptionKey = configuration["Encryption:Key"] ?? throw new InvalidOperationException("Encryption key not configured");
        var encryptionIV = configuration["Encryption:IV"] ?? throw new InvalidOperationException("Encryption IV not configured");

        _key = Convert.FromBase64String(encryptionKey);
        _iv = Convert.FromBase64String(encryptionIV);

        if (_key.Length != 32)
            throw new InvalidOperationException("Encryption key must be 32 bytes (256 bits) for AES-256");

        if (_iv.Length != 16)
            throw new InvalidOperationException("Encryption IV must be 16 bytes (128 bits)");
    }

    public string Encrypt(string plainText)
    {
        if (string.IsNullOrEmpty(plainText))
            throw new ArgumentException("Plain text cannot be empty", nameof(plainText));

        using var aes = Aes.Create();
        aes.Key = _key;
        aes.IV = _iv;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;

        using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
        using var msEncrypt = new MemoryStream();
        using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
        using (var swEncrypt = new StreamWriter(csEncrypt))
        {
            swEncrypt.Write(plainText);
        }

        return Convert.ToBase64String(msEncrypt.ToArray());
    }

    public string Decrypt(string cipherText)
    {
        if (string.IsNullOrEmpty(cipherText))
            throw new ArgumentException("Cipher text cannot be empty", nameof(cipherText));

        var buffer = Convert.FromBase64String(cipherText);

        using var aes = Aes.Create();
        aes.Key = _key;
        aes.IV = _iv;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;

        using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
        using var msDecrypt = new MemoryStream(buffer);
        using var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
        using var srDecrypt = new StreamReader(csDecrypt);

        return srDecrypt.ReadToEnd();
    }

    public string GenerateRandomSecret(int length = 32)
    {
        const string validChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567"; // Base32 alphabet
        var random = new byte[length];
        
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(random);
        }

        var result = new StringBuilder(length);
        foreach (var b in random)
        {
            result.Append(validChars[b % validChars.Length]);
        }

        return result.ToString();
    }
}
