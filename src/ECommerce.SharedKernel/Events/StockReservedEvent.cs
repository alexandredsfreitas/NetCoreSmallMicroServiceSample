namespace ECommerce.SharedKernel.Events;

public record StockReservedEvent(
    Guid EventId, 
    DateTime OccurredOn, 
    Guid OrderId, 
    Guid ProductId, 
    int Quantity
) : DomainEvent(EventId, OccurredOn);