namespace ECommerce.SharedKernel.Events;

public record StockReservationFailedEvent(
    Guid EventId, 
    DateTime OccurredOn, 
    Guid OrderId, 
    Guid ProductId, 
    int RequestedQuantity,
    string Reason
) : DomainEvent(EventId, OccurredOn);