namespace ECommerce.SharedKernel.Events;

public record OrderCreatedEvent(
    Guid EventId, 
    DateTime OccurredOn, 
    Guid OrderId, 
    Guid ProductId, 
    int Quantity
) : DomainEvent(EventId, OccurredOn);