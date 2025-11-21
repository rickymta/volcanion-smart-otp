using MediatR;

namespace SmartOTP.Application.Features.OtpAccounts.Commands;

public class DeleteOtpAccountCommand : IRequest<bool>
{
    public Guid UserId { get; set; }
    public Guid AccountId { get; set; }
}
