using ECommerce.Infrastructure.Messaging;
using ECommerce.Orders.Application.Commands;
using ECommerce.Orders.Domain.Models;
using ECommerce.Orders.Domain.Repository.Interfaces;
using MediatR;

namespace ECommerce.Orders.Application.CommandHandlers;

public class CreateOrderHandler : IRequestHandler<CreateOrderCommand, Guid>
{
    private readonly IOrderRepository _repository;
    private readonly IEventBus _eventBus;
    
    public CreateOrderHandler(IOrderRepository repository, IEventBus eventBus)
    {
        _repository = repository;
        _eventBus = eventBus;
    }
    
    public async Task<Guid> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var order = new Order(new ProductId(request.ProductId), request.Quantity);
            
        await _repository.AddAsync(order);
            
        // Publicar eventos de domínio
        foreach (var domainEvent in order.Events)
        {
            await _eventBus.PublishAsync(domainEvent);
        }
            
        order.ClearEvents();
            
        return order.Id.Value;
    }
}