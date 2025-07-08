using ECommerce.Orders.Application.DTOs;
using ECommerce.Orders.Application.Queries;
using ECommerce.Orders.Domain.Models;
using ECommerce.Orders.Domain.Repository.Interfaces;
using MediatR;

namespace ECommerce.Orders.Application.QueryHandlers;

public class GetOrderHandler : IRequestHandler<GetOrderQuery, OrderDto>
{
    private readonly IOrderRepository _repository;
        
    public GetOrderHandler(IOrderRepository repository)
    {
        _repository = repository;
    }
    
    public async Task<OrderDto> Handle(GetOrderQuery request, CancellationToken cancellationToken)
    {
        var order = await _repository.GetByIdAsync(new OrderId(request.OrderId));
            
        if (order == null)
            throw new NotFoundException($"Pedido {request.OrderId} não encontrado");
                
        return new OrderDto(
            order.Id.Value,
            order.ProductId.Value,
            order.Quantity,
            order.Status.ToString(),
            order.CreatedAt);
    }
}