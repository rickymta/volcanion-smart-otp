using MediatR;
using SmartOTP.Application.Common.Interfaces;
using SmartOTP.Domain.Entities;
using SmartOTP.Domain.Enums;

namespace SmartOTP.Application.Features.OtpAccounts.Commands;

public class DeleteOtpAccountCommandHandler(
    IRepository<OtpAccount> otpAccountRepository,
    IAuditService auditService,
    IUnitOfWork unitOfWork) : IRequestHandler<DeleteOtpAccountCommand, bool>
{
    public async Task<bool> Handle(DeleteOtpAccountCommand request, CancellationToken cancellationToken)
    {
        var account = await otpAccountRepository.FirstOrDefaultAsync(a => a.Id == request.AccountId && a.UserId == request.UserId, cancellationToken)
            ?? throw new InvalidOperationException("OTP account not found or access denied");

        account.Delete();
        await unitOfWork.SaveChangesAsync(cancellationToken);

        await auditService.LogAsync(
            AuditLog.CreateSuccess(
                request.UserId,
                AuditActionType.OtpAccountDeleted,
                details: $"{account.Issuer} - {account.AccountName}"),
            cancellationToken);

        return true;
    }
}
