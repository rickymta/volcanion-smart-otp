using MediatR;

namespace SmartOTP.Domain.Common;

public interface IDomainEvent : INotification
{
    DateTime OccurredOn { get; }
}
