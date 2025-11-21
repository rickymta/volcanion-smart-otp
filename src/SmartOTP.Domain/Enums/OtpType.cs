using System.ComponentModel;

namespace SmartOTP.Domain.Enums;

public enum OtpType
{
    /// <summary>
    /// Time-based OTP (RFC 6238)
    /// </summary>
    [Description("Time-based OTP")]
    TOTP = 1,

    /// <summary>
    /// Counter-based OTP (RFC 4226)
    /// </summary>
    [Description("Counter-based OTP")]
    HOTP = 2
}
