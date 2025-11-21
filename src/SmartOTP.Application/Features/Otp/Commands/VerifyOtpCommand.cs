using MediatR;

namespace SmartOTP.Application.Features.Otp.Commands;

public class VerifyOtpCommand : IRequest<bool>
{
    public Guid UserId { get; set; }
    public Guid AccountId { get; set; }
    public string Code { get; set; } = null!;
}
