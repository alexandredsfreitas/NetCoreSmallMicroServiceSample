using ECommerce.SharedKernel.Events;

namespace ECommerce.Orders.Domain.Models;

public class Order
{
    public OrderId Id { get; private set; }
    public ProductId ProductId { get; private set; }
    public int Quantity { get; private set; }
    public OrderStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
        
    private readonly List<DomainEvent> _events = new();
    public IReadOnlyList<DomainEvent> Events => _events.AsReadOnly();
        
    private Order() { } // EF Constructor
    
    public Order(ProductId productId, int quantity)
    {
        Id = new OrderId(Guid.NewGuid());
        ProductId = productId;
        Quantity = quantity;
        Status = OrderStatus.Pending;
        CreatedAt = DateTime.UtcNow;
            
        _events.Add(new OrderCreatedEvent(
            Guid.NewGuid(), 
            DateTime.UtcNow, 
            Id.Value, 
            ProductId.Value, 
            Quantity));
    }
    
    public void Confirm()
    {
        if (Status != OrderStatus.Pending)
            throw new InvalidOperationException("Apenas pedidos pendentes podem ser confirmados");
                
        Status = OrderStatus.Confirmed;
    }
        
    public void Cancel()
    {
        if (Status == OrderStatus.Confirmed)
            throw new InvalidOperationException("Pedidos confirmados não podem ser cancelados");
                
        Status = OrderStatus.Cancelled;
    }
        
    public void ClearEvents() => _events.Clear();
}