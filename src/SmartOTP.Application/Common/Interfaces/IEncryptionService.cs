namespace SmartOTP.Application.Common.Interfaces;

public interface IEncryptionService
{
    string Encrypt(string plainText);
    string Decrypt(string cipherText);
    string GenerateRandomSecret(int length = 32);
}
