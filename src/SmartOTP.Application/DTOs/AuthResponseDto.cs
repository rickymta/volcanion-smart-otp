namespace SmartOTP.Application.DTOs;

public class AuthResponseDto
{
    public string AccessToken { get; set; } = null!;
    public string RefreshToken { get; set; } = null!;
    public DateTime AccessTokenExpiresAt { get; set; }
    public DateTime RefreshTokenExpiresAt { get; set; }
    public UserDto User { get; set; } = null!;
}
