namespace LineControlCenter.Domain.Primitives;

/// <summary>Base class for all domain entities with a strongly-typed identifier.</summary>
public abstract class Entity<TId>
{
    private readonly List<IDomainEvent> _domainEvents = [];

    /// <summary>The entity's unique identifier.</summary>
    public TId Id { get; protected set; } = default!;

    /// <summary>Domain events raised by this entity during the current operation.</summary>
    public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    /// <summary>Raises a domain event to be dispatched on save.</summary>
    protected void RaiseDomainEvent(IDomainEvent @event) => _domainEvents.Add(@event);

    /// <summary>Clears all pending domain events (called after dispatch).</summary>
    public void ClearDomainEvents() => _domainEvents.Clear();
}
