using SmartOTP.Domain.Common;

namespace SmartOTP.Domain.Events;

public class OtpAccountDeletedEvent(Guid accountId, Guid userId) : IDomainEvent
{
    public Guid AccountId { get; } = accountId;
    public Guid UserId { get; } = userId;
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
