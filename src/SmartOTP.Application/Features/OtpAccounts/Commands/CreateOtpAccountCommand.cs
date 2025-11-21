using MediatR;
using SmartOTP.Application.DTOs;
using SmartOTP.Domain.Enums;

namespace SmartOTP.Application.Features.OtpAccounts.Commands;

public class CreateOtpAccountCommand : IRequest<OtpAccountDto>
{
    public Guid UserId { get; set; }
    public string Issuer { get; set; } = null!;
    public string AccountName { get; set; } = null!;
    public OtpType Type { get; set; }
    public string? Secret { get; set; } // If null, generate random
    public OtpAlgorithm Algorithm { get; set; } = OtpAlgorithm.SHA1;
    public int Digits { get; set; } = 6;
    public int Period { get; set; } = 30;
    public long Counter { get; set; } = 0;
    public string? IconUrl { get; set; }
}
