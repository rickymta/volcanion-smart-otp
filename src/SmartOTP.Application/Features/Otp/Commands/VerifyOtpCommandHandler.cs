using MediatR;
using SmartOTP.Application.Common.Interfaces;
using SmartOTP.Domain.Entities;
using SmartOTP.Domain.Enums;

namespace SmartOTP.Application.Features.Otp.Commands;

public class VerifyOtpCommandHandler(
    IRepository<OtpAccount> otpAccountRepository,
    IEncryptionService encryptionService,
    IOtpService otpService,
    ICacheService cacheService,
    IAuditService auditService) : IRequestHandler<VerifyOtpCommand, bool>
{
    public async Task<bool> Handle(VerifyOtpCommand request, CancellationToken cancellationToken)
    {
        // Rate limiting: Check attempts
        var rateLimitKey = $"otp_verify_attempts:{request.UserId}:{request.AccountId}";
        var attempts = await cacheService.IncrementAsync(rateLimitKey, 1, TimeSpan.FromMinutes(5), cancellationToken);

        if (attempts > 5)
        {
            await auditService.LogAsync(
                AuditLog.CreateFailure(
                    request.UserId,
                    AuditActionType.OtpVerificationFailed,
                    "Rate limit exceeded",
                    details: $"Account: {request.AccountId}"),
                cancellationToken);
            throw new InvalidOperationException("Too many verification attempts. Please try again later.");
        }

        var account = await otpAccountRepository.FirstOrDefaultAsync(a => a.Id == request.AccountId && a.UserId == request.UserId && !a.IsDeleted, cancellationToken)
            ?? throw new InvalidOperationException("OTP account not found or access denied");

        // Decrypt secret
        var plainSecret = encryptionService.Decrypt(account.Secret.EncryptedValue);

        // Verify OTP code
        bool isValid;
        if (account.Type == OtpType.TOTP)
        {
            isValid = otpService.VerifyTOTP(plainSecret, request.Code, account.Digits, account.Period, account.Algorithm);
        }
        else
        {
            isValid = otpService.VerifyHOTP(plainSecret, request.Code, account.Counter, account.Digits, account.Algorithm);
        }

        // Log verification
        if (isValid)
        {
            await cacheService.RemoveAsync(rateLimitKey, cancellationToken); // Reset rate limit on success
            await auditService.LogAsync(
                AuditLog.CreateSuccess(
                    request.UserId,
                    AuditActionType.OtpVerified,
                    details: $"{account.Issuer} - {account.AccountName}"),
                cancellationToken);
        }
        else
        {
            await auditService.LogAsync(
                AuditLog.CreateFailure(
                    request.UserId,
                    AuditActionType.OtpVerificationFailed,
                    "Invalid OTP code",
                    details: $"{account.Issuer} - {account.AccountName}"),
                cancellationToken);
        }

        return isValid;
    }
}
