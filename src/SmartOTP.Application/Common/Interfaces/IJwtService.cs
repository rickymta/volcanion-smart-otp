namespace SmartOTP.Application.Common.Interfaces;

public interface IJwtService
{
    string GenerateAccessToken(Guid userId, string email);
    string GenerateRefreshToken();
    bool ValidateToken(string token, out Guid userId);
}
