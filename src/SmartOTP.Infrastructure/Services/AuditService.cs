using SmartOTP.Application.Common.Interfaces;
using SmartOTP.Domain.Entities;

namespace SmartOTP.Infrastructure.Services;

public class AuditService(IRepository<AuditLog> auditLogRepository, IUnitOfWork unitOfWork) : IAuditService
{
    public async Task LogAsync(AuditLog auditLog, CancellationToken cancellationToken = default)
    {
        await auditLogRepository.AddAsync(auditLog, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
