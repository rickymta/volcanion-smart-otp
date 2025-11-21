using SmartOTP.Domain.Enums;

namespace SmartOTP.Application.DTOs;

public class OtpAccountDto
{
    public Guid Id { get; set; }
    public string Issuer { get; set; } = null!;
    public string AccountName { get; set; } = null!;
    public OtpType Type { get; set; }
    public OtpAlgorithm Algorithm { get; set; }
    public int Digits { get; set; }
    public int Period { get; set; }
    public long Counter { get; set; }
    public string? IconUrl { get; set; }
    public int SortOrder { get; set; }
    public DateTime CreatedAt { get; set; }
}
