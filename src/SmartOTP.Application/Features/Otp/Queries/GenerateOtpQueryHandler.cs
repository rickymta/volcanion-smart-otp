using MediatR;
using SmartOTP.Application.Common.Interfaces;
using SmartOTP.Application.DTOs;
using SmartOTP.Domain.Entities;
using SmartOTP.Domain.Enums;

namespace SmartOTP.Application.Features.Otp.Queries;

public class GenerateOtpQueryHandler(
    IRepository<OtpAccount> otpAccountRepository,
    IEncryptionService encryptionService,
    IOtpService otpService,
    IAuditService auditService,
    IUnitOfWork unitOfWork) : IRequestHandler<GenerateOtpQuery, OtpCodeDto>
{
    public async Task<OtpCodeDto> Handle(GenerateOtpQuery request, CancellationToken cancellationToken)
    {
        var account = await otpAccountRepository.FirstOrDefaultAsync(a => a.Id == request.AccountId && a.UserId == request.UserId && !a.IsDeleted, cancellationToken)
            ?? throw new InvalidOperationException("OTP account not found or access denied");

        // Decrypt secret
        var plainSecret = encryptionService.Decrypt(account.Secret.EncryptedValue);

        // Generate OTP code
        string code;
        if (account.Type == OtpType.TOTP)
        {
            code = otpService.GenerateTOTP(plainSecret, account.Digits, account.Period, account.Algorithm);
        }
        else
        {
            code = otpService.GenerateHOTP(plainSecret, account.Counter, account.Digits, account.Algorithm);
            account.IncrementCounter();
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }

        // Log generation
        await auditService.LogAsync(
            AuditLog.CreateSuccess(
                request.UserId,
                AuditActionType.OtpGenerated,
                details: $"{account.Issuer} - {account.AccountName}"),
            cancellationToken);

        return new OtpCodeDto
        {
            Code = code,
            RemainingSeconds = account.Type == OtpType.TOTP ? otpService.GetRemainingSeconds(account.Period) : 0,
            GeneratedAt = DateTime.UtcNow
        };
    }
}
