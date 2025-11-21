using FluentValidation;

namespace SmartOTP.Application.Features.Otp.Commands;

public class VerifyOtpCommandValidator : AbstractValidator<VerifyOtpCommand>
{
    public VerifyOtpCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required");

        RuleFor(x => x.AccountId)
            .NotEmpty().WithMessage("Account ID is required");

        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("OTP code is required")
            .Matches(@"^\d{6,8}$").WithMessage("OTP code must be 6 or 8 digits");
    }
}
