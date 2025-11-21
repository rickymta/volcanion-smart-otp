using SmartOTP.Domain.Common;
using SmartOTP.Domain.Enums;

namespace SmartOTP.Domain.Entities;

public class AuditLog : BaseEntity
{
    public Guid? UserId { get; private set; }
    public AuditActionType Action { get; private set; }
    public string? IpAddress { get; private set; }
    public string? UserAgent { get; private set; }
    public string? Details { get; private set; }
    public bool IsSuccess { get; private set; }
    public string? ErrorMessage { get; private set; }

    // Navigation properties
    public User? User { get; private set; }

    private AuditLog() { } // EF Core constructor

    private AuditLog(
        Guid? userId,
        AuditActionType action,
        bool isSuccess,
        string? ipAddress = null,
        string? userAgent = null,
        string? details = null,
        string? errorMessage = null)
    {
        UserId = userId;
        Action = action;
        IsSuccess = isSuccess;
        IpAddress = ipAddress;
        UserAgent = userAgent;
        Details = details;
        ErrorMessage = errorMessage;
    }

    public static AuditLog CreateSuccess(
        Guid? userId,
        AuditActionType action,
        string? ipAddress = null,
        string? userAgent = null,
        string? details = null)
    {
        return new AuditLog(userId, action, true, ipAddress, userAgent, details);
    }

    public static AuditLog CreateFailure(
        Guid? userId,
        AuditActionType action,
        string errorMessage,
        string? ipAddress = null,
        string? userAgent = null,
        string? details = null)
    {
        if (string.IsNullOrWhiteSpace(errorMessage))
            throw new ArgumentException("Error message cannot be empty for failed audit logs", nameof(errorMessage));

        return new AuditLog(userId, action, false, ipAddress, userAgent, details, errorMessage);
    }
}
