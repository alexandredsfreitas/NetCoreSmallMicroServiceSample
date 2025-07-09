using ECommerce.SharedKernel.Events;

namespace ECommerce.Stock.Domain.Models;

public class Product
{
    public ProductId Id { get; private set; }
    public string Name { get; private set; }
    public StockQuantity AvailableQuantity { get; private set; }
    public StockQuantity ReservedQuantity { get; private set; }
        
    private readonly List<DomainEvent> _events = new();
    public IReadOnlyList<DomainEvent> Events => _events.AsReadOnly();
        
    private Product() { } // EF Constructor
        
    public Product(ProductId id, string name, StockQuantity initialQuantity)
    {
        Id = id;
        Name = name;
        AvailableQuantity = initialQuantity;
        ReservedQuantity = new StockQuantity(0);
    }
        
    public bool CanReserve(int quantity)
    {
        return AvailableQuantity >= quantity;
    }
        
    public void ReserveStock(Guid orderId, int quantity)
    {
        if (!CanReserve(quantity))
        {
            _events.Add(new StockReservationFailedEvent(
                Guid.NewGuid(),
                DateTime.UtcNow,
                orderId,
                Id.Value,
                quantity,
                $"Estoque insuficiente. Disponível: {AvailableQuantity}, Solicitado: {quantity}"));
            return;
        }
            
        AvailableQuantity = new StockQuantity(AvailableQuantity - quantity);
        ReservedQuantity = new StockQuantity(ReservedQuantity + quantity);
            
        _events.Add(new StockReservedEvent(
            Guid.NewGuid(),
            DateTime.UtcNow,
            orderId,
            Id.Value,
            quantity));
    }
        
    public void AddStock(int quantity)
    {
        AvailableQuantity = new StockQuantity(AvailableQuantity + quantity);
    }
        
    public void ClearEvents() => _events.Clear();
}