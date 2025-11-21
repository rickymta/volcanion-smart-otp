using MediatR;
using SmartOTP.Application.Common.Interfaces;
using SmartOTP.Application.DTOs;
using SmartOTP.Domain.Entities;

namespace SmartOTP.Application.Features.OtpAccounts.Queries;

public class GetUserOtpAccountsQueryHandler(IRepository<OtpAccount> otpAccountRepository) : IRequestHandler<GetUserOtpAccountsQuery, IEnumerable<OtpAccountDto>>
{
    public async Task<IEnumerable<OtpAccountDto>> Handle(GetUserOtpAccountsQuery request, CancellationToken cancellationToken)
    {
        var accounts = await otpAccountRepository.FindAsync(
            a => a.UserId == request.UserId && !a.IsDeleted,
            cancellationToken);

        return accounts
            .OrderBy(a => a.SortOrder)
            .ThenBy(a => a.CreatedAt)
            .Select(a => new OtpAccountDto
            {
                Id = a.Id,
                Issuer = a.Issuer,
                AccountName = a.AccountName,
                Type = a.Type,
                Algorithm = a.Algorithm,
                Digits = a.Digits,
                Period = a.Period,
                Counter = a.Counter,
                IconUrl = a.IconUrl,
                SortOrder = a.SortOrder,
                CreatedAt = a.CreatedAt
            })
            .ToList();
    }
}
