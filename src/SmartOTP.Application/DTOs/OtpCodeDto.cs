namespace SmartOTP.Application.DTOs;

public class OtpCodeDto
{
    public string Code { get; set; } = null!;
    public int RemainingSeconds { get; set; }
    public DateTime GeneratedAt { get; set; }
}
