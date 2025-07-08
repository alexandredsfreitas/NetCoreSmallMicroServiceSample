using ECommerce.Orders.Domain.Models;
using ECommerce.Orders.Domain.Repository.Interfaces;
using ECommerce.SharedKernel.Events;
using Microsoft.Extensions.Logging;

namespace ECommerce.Orders.Application.EventHandlers;

public class StockReservationFailedEventHandler
{
    private readonly IOrderRepository _repository;
    private readonly ILogger<StockReservationFailedEventHandler> _logger;
        
    public StockReservationFailedEventHandler(IOrderRepository repository, ILogger<StockReservationFailedEventHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }
        
    public async Task Handle(StockReservationFailedEvent @event)
    {
        var order = await _repository.GetByIdAsync(new OrderId(@event.OrderId));
        if (order != null)
        {
            order.Cancel();
            await _repository.UpdateAsync(order);
            _logger.LogInformation("Pedido {OrderId} cancelado: {Reason}", @event.OrderId, @event.Reason);
        }
    }
}