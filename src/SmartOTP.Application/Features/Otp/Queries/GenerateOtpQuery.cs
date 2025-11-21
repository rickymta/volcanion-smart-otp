using MediatR;
using SmartOTP.Application.DTOs;

namespace SmartOTP.Application.Features.Otp.Queries;

public class GenerateOtpQuery : IRequest<OtpCodeDto>
{
    public Guid UserId { get; set; }
    public Guid AccountId { get; set; }
}
