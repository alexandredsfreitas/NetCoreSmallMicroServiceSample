using ECommerce.Orders.Domain.Models;
using ECommerce.Orders.Domain.Repository.Interfaces;
using ECommerce.SharedKernel.Events;
using Microsoft.Extensions.Logging;

namespace ECommerce.Orders.Application.EventHandlers;

public class StockReservedEventHandler
{
    private readonly IOrderRepository _repository;
    private readonly ILogger<StockReservedEventHandler> _logger;
        
    public StockReservedEventHandler(IOrderRepository repository, ILogger<StockReservedEventHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }
        
    public async Task Handle(StockReservedEvent @event)
    {
        var order = await _repository.GetByIdAsync(new OrderId(@event.OrderId));
        if (order != null)
        {
            order.Confirm();
            await _repository.UpdateAsync(order);
            _logger.LogInformation("Pedido {OrderId} confirmado", @event.OrderId);
        }
    }
}