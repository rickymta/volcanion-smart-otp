using MediatR;
using SmartOTP.Application.DTOs;

namespace SmartOTP.Application.Features.OtpAccounts.Queries;

public class GetUserOtpAccountsQuery : IRequest<IEnumerable<OtpAccountDto>>
{
    public Guid UserId { get; set; }
}
