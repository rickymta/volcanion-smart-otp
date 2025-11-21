using SmartOTP.Domain.Enums;

namespace SmartOTP.Application.Common.Interfaces;

public interface IOtpService
{
    string GenerateTOTP(string secret, int digits = 6, int period = 30, OtpAlgorithm algorithm = OtpAlgorithm.SHA1);
    string GenerateHOTP(string secret, long counter, int digits = 6, OtpAlgorithm algorithm = OtpAlgorithm.SHA1);
    bool VerifyTOTP(string secret, string code, int digits = 6, int period = 30, OtpAlgorithm algorithm = OtpAlgorithm.SHA1, int window = 1);
    bool VerifyHOTP(string secret, string code, long counter, int digits = 6, OtpAlgorithm algorithm = OtpAlgorithm.SHA1);
    int GetRemainingSeconds(int period = 30);
}
