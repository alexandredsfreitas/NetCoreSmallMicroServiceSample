using ECommerce.Infrastructure.Messaging;
using ECommerce.SharedKernel.Events;
using ECommerce.Stock.Domain.Models;
using ECommerce.Stock.Domain.Repository;
using Microsoft.Extensions.Logging;

namespace ECommerce.Stock.Application.EventHandlers;

public class OrderCreatedEventHandler
{
    private readonly IProductRepository _repository;
    private readonly IEventBus _eventBus;
    private readonly ILogger<OrderCreatedEventHandler> _logger;
        
    public OrderCreatedEventHandler(
        IProductRepository repository, 
        IEventBus eventBus,
        ILogger<OrderCreatedEventHandler> logger)
    {
        _repository = repository;
        _eventBus = eventBus;
        _logger = logger;
    }
        
    public async Task Handle(OrderCreatedEvent @event)
    {
        _logger.LogInformation("Processando reserva de estoque para pedido {OrderId}", @event.OrderId);
            
        var product = await _repository.GetByIdAsync(new ProductId(@event.ProductId));
        if (product == null)
        {
            _logger.LogWarning("Produto {ProductId} não encontrado", @event.ProductId);
            return;
        }
            
        product.ReserveStock(@event.OrderId, @event.Quantity);
        await _repository.UpdateAsync(product);
            
        // Publicar eventos resultantes
        foreach (var domainEvent in product.Events)
        {
            await _eventBus.PublishAsync(domainEvent);
        }
            
        product.ClearEvents();
    }
}