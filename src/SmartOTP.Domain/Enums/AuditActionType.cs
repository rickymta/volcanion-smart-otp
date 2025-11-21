using System.ComponentModel;

namespace SmartOTP.Domain.Enums;

public enum AuditActionType
{
    [Description("User Registered")]
    UserRegistered = 1,

    [Description("User Logged In")]
    UserLoggedIn = 2,

    [Description("User Logged Out")]
    UserLoggedOut = 3,

    [Description("OTP Account Created")]
    OtpAccountCreated = 4,

    [Description("OTP Account Updated")]
    OtpAccountUpdated = 5,

    [Description("OTP Account Deleted")]
    OtpAccountDeleted = 6,

    [Description("OTP Generated")]
    OtpGenerated = 7,

    [Description("OTP Verified")]
    OtpVerified = 8,

    [Description("OTP Verification Failed")]
    OtpVerificationFailed = 9,

    [Description("Push OTP Approved")]
    PushOtpApproved = 10,

    [Description("Push OTP Rejected")]
    PushOtpRejected = 11,

    [Description("Refresh Token Used")]
    RefreshTokenUsed = 12
}
