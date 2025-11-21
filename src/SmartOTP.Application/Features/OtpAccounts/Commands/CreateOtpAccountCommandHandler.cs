using MediatR;
using SmartOTP.Application.Common.Interfaces;
using SmartOTP.Application.DTOs;
using SmartOTP.Domain.Entities;
using SmartOTP.Domain.Enums;
using SmartOTP.Domain.ValueObjects;

namespace SmartOTP.Application.Features.OtpAccounts.Commands;

public class CreateOtpAccountCommandHandler(
    IRepository<User> userRepository,
    IRepository<OtpAccount> otpAccountRepository,
    IEncryptionService encryptionService,
    IAuditService auditService,
    IUnitOfWork unitOfWork) : IRequestHandler<CreateOtpAccountCommand, OtpAccountDto>
{
    public async Task<OtpAccountDto> Handle(CreateOtpAccountCommand request, CancellationToken cancellationToken)
    {
        // Verify user exists
        var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user == null)
        {
            throw new InvalidOperationException("User not found");
        }

        // Generate or use provided secret
        var plainSecret = request.Secret ?? encryptionService.GenerateRandomSecret();
        var encryptedSecret = encryptionService.Encrypt(plainSecret);
        var secretKey = SecretKey.Create(encryptedSecret);

        // Create OTP account
        OtpAccount otpAccount;
        if (request.Type == OtpType.TOTP)
        {
            otpAccount = OtpAccount.CreateTOTP(
                request.UserId,
                request.Issuer,
                request.AccountName,
                secretKey,
                request.Algorithm,
                request.Digits,
                request.Period,
                request.IconUrl);
        }
        else
        {
            otpAccount = OtpAccount.CreateHOTP(
                request.UserId,
                request.Issuer,
                request.AccountName,
                secretKey,
                request.Algorithm,
                request.Digits,
                request.Counter,
                request.IconUrl);
        }

        await otpAccountRepository.AddAsync(otpAccount, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        await auditService.LogAsync(
            AuditLog.CreateSuccess(
                request.UserId,
                AuditActionType.OtpAccountCreated,
                details: $"{request.Issuer} - {request.AccountName}"),
            cancellationToken);

        return new OtpAccountDto
        {
            Id = otpAccount.Id,
            Issuer = otpAccount.Issuer,
            AccountName = otpAccount.AccountName,
            Type = otpAccount.Type,
            Algorithm = otpAccount.Algorithm,
            Digits = otpAccount.Digits,
            Period = otpAccount.Period,
            Counter = otpAccount.Counter,
            IconUrl = otpAccount.IconUrl,
            SortOrder = otpAccount.SortOrder,
            CreatedAt = otpAccount.CreatedAt
        };
    }
}
