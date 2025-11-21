using System.ComponentModel;

namespace SmartOTP.Domain.Enums;

public enum OtpAlgorithm
{
    [Description("SHA-1")]
    SHA1 = 1,

    [Description("SHA-256")]
    SHA256 = 2,

    [Description("SHA-512")]
    SHA512 = 3
}
