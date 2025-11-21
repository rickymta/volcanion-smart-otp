namespace SmartOTP.Domain.Common;

public abstract class BaseEntity
{
    public Guid Id { get; protected set; }
    public DateTime CreatedAt { get; protected set; }
    public DateTime? UpdatedAt { get; protected set; }
    public bool IsDeleted { get; protected set; }

    private readonly List<IDomainEvent> _domainEvents = [];
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected BaseEntity()
    {
        Id = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
        IsDeleted = false;
    }

    public void AddDomainEvent(IDomainEvent eventItem)
    {
        _domainEvents.Add(eventItem);
    }

    public void RemoveDomainEvent(IDomainEvent eventItem)
    {
        _domainEvents.Remove(eventItem);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    public void MarkAsUpdated()
    {
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsDeleted()
    {
        IsDeleted = true;
        UpdatedAt = DateTime.UtcNow;
    }
}
