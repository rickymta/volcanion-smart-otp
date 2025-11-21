using SmartOTP.Domain.Common;

namespace SmartOTP.Domain.Events;

public class OtpGeneratedEvent(Guid accountId, Guid userId, string code) : IDomainEvent
{
    public Guid AccountId { get; } = accountId;
    public Guid UserId { get; } = userId;
    public string Code { get; } = code;
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
