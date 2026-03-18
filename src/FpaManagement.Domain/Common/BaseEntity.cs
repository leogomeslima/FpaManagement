namespace FpaManagement.Domain.Common;

public abstract class BaseEntity
{
    public Guid Id
    {
        get; protected set;
    }
    public DateTime CreatedAt
    {
        get; set;
    }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime? UpdatedAt
    {
        get; set;
    }
    public string? UpdatedBy
    {
        get; set;
    }
    public DateTime? DeletedAt
    {
        get; set;
    }
    public string? DeletedBy
    {
        get; set;
    }
    public bool IsDeleted
    {
        get; set;
    }

    protected BaseEntity()
    {
        Id = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
    }
}

public abstract class BaseAuditableEntity : BaseEntity
{
    private readonly List<DomainEvent> _domainEvents = new();

    public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    public void AddDomainEvent(DomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public void RemoveDomainEvent(DomainEvent domainEvent)
    {
        _domainEvents.Remove(domainEvent);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}

public abstract class DomainEvent
{
    public DateTime OccurredOn { get; protected set; } = DateTime.UtcNow;
}
