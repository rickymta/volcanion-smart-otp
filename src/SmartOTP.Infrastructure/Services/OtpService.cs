using System.Security.Cryptography;
using System.Text;
using SmartOTP.Application.Common.Interfaces;
using SmartOTP.Domain.Enums;

namespace SmartOTP.Infrastructure.Services;

public class OtpService : IOtpService
{
    public string GenerateTOTP(string secret, int digits = 6, int period = 30, OtpAlgorithm algorithm = OtpAlgorithm.SHA1)
    {
        var unixTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var counter = unixTimestamp / period;
        return GenerateOtp(secret, counter, digits, algorithm);
    }

    public string GenerateHOTP(string secret, long counter, int digits = 6, OtpAlgorithm algorithm = OtpAlgorithm.SHA1)
    {
        return GenerateOtp(secret, counter, digits, algorithm);
    }

    public bool VerifyTOTP(string secret, string code, int digits = 6, int period = 30, OtpAlgorithm algorithm = OtpAlgorithm.SHA1, int window = 1)
    {
        var unixTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var currentCounter = unixTimestamp / period;

        // Check current time window and adjacent windows
        for (int i = -window; i <= window; i++)
        {
            var otp = GenerateOtp(secret, currentCounter + i, digits, algorithm);
            if (otp == code)
            {
                return true;
            }
        }

        return false;
    }

    public bool VerifyHOTP(string secret, string code, long counter, int digits = 6, OtpAlgorithm algorithm = OtpAlgorithm.SHA1)
    {
        var otp = GenerateOtp(secret, counter, digits, algorithm);
        return otp == code;
    }

    public int GetRemainingSeconds(int period = 30)
    {
        var unixTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var remaining = period - (unixTimestamp % period);
        return (int)remaining;
    }

    private string GenerateOtp(string secret, long counter, int digits, OtpAlgorithm algorithm)
    {
        // Convert Base32 secret to bytes
        var secretBytes = Base32Decode(secret);

        // Convert counter to byte array (big-endian)
        var counterBytes = BitConverter.GetBytes(counter);
        if (BitConverter.IsLittleEndian)
        {
            Array.Reverse(counterBytes);
        }

        // Compute HMAC
        byte[] hash;
        switch (algorithm)
        {
            case OtpAlgorithm.SHA256:
                hash = HMACSHA256.HashData(secretBytes, counterBytes);
                break;
            case OtpAlgorithm.SHA512:
                hash = HMACSHA512.HashData(secretBytes, counterBytes);
                break;
            default: // SHA1
                hash = HMACSHA1.HashData(secretBytes, counterBytes);
                break;
        }

        // Dynamic truncation
        var offset = hash[^1] & 0x0F;
        var binary =
            ((hash[offset] & 0x7F) << 24) |
            ((hash[offset + 1] & 0xFF) << 16) |
            ((hash[offset + 2] & 0xFF) << 8) |
            (hash[offset + 3] & 0xFF);

        var otp = binary % (int)Math.Pow(10, digits);
        return otp.ToString($"D{digits}");
    }

    private byte[] Base32Decode(string base32)
    {
        const string base32Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";
        base32 = base32.TrimEnd('=').ToUpperInvariant();

        var bits = new StringBuilder();
        foreach (var c in base32)
        {
            var value = base32Chars.IndexOf(c);
            if (value < 0)
                throw new ArgumentException("Invalid Base32 character", nameof(base32));

            bits.Append(Convert.ToString(value, 2).PadLeft(5, '0'));
        }

        var byteCount = bits.Length / 8;
        var bytes = new byte[byteCount];

        for (int i = 0; i < byteCount; i++)
        {
            bytes[i] = Convert.ToByte(bits.ToString(i * 8, 8), 2);
        }

        return bytes;
    }
}
