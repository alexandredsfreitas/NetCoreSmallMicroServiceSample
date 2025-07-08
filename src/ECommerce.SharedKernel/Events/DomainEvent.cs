namespace ECommerce.SharedKernel.Events;

public abstract record DomainEvent(Guid EventId, DateTime OccurredOn);