using SmartOTP.Domain.Common;

namespace SmartOTP.Domain.Events;

public class OtpVerifiedEvent(Guid accountId, Guid userId, bool isValid) : IDomainEvent
{
    public Guid AccountId { get; } = accountId;
    public Guid UserId { get; } = userId;
    public bool IsValid { get; } = isValid;
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
