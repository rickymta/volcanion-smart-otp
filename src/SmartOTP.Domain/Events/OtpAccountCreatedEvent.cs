using SmartOTP.Domain.Common;

namespace SmartOTP.Domain.Events;

public class OtpAccountCreatedEvent(Guid accountId, Guid userId, string issuer, string accountName) : IDomainEvent
{
    public Guid AccountId { get; } = accountId;
    public Guid UserId { get; } = userId;
    public string Issuer { get; } = issuer;
    public string AccountName { get; } = accountName;
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
