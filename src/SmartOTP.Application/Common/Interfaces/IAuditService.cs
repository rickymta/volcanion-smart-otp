using SmartOTP.Domain.Entities;

namespace SmartOTP.Application.Common.Interfaces;

public interface IAuditService
{
    Task LogAsync(AuditLog auditLog, CancellationToken cancellationToken = default);
}
