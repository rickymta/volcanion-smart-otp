using FluentValidation;
using SmartOTP.Domain.Enums;

namespace SmartOTP.Application.Features.OtpAccounts.Commands;

public class CreateOtpAccountCommandValidator : AbstractValidator<CreateOtpAccountCommand>
{
    public CreateOtpAccountCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required");

        RuleFor(x => x.Issuer)
            .NotEmpty().WithMessage("Issuer is required")
            .MaximumLength(100).WithMessage("Issuer must not exceed 100 characters");

        RuleFor(x => x.AccountName)
            .NotEmpty().WithMessage("Account name is required")
            .MaximumLength(100).WithMessage("Account name must not exceed 100 characters");

        RuleFor(x => x.Type)
            .IsInEnum().WithMessage("Invalid OTP type");

        RuleFor(x => x.Algorithm)
            .IsInEnum().WithMessage("Invalid algorithm");

        RuleFor(x => x.Digits)
            .Must(d => d == 6 || d == 8).WithMessage("Digits must be 6 or 8");

        RuleFor(x => x.Period)
            .GreaterThan(0).WithMessage("Period must be greater than 0")
            .When(x => x.Type == OtpType.TOTP);

        RuleFor(x => x.Counter)
            .GreaterThanOrEqualTo(0).WithMessage("Counter must be non-negative")
            .When(x => x.Type == OtpType.HOTP);
    }
}
