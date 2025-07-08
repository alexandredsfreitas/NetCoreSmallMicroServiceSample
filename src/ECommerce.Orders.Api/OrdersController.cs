using ECommerce.Orders.Application.Commands;
using ECommerce.Orders.Application.DTOs;
using ECommerce.Orders.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Orders.Api;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;
        
    public OrdersController(IMediator mediator)
    {
        _mediator = mediator;
    }
        
    [HttpPost]
    public async Task<ActionResult<Guid>> CreateOrder([FromBody] CreateOrderCommand command)
    {
        var orderId = await _mediator.Send(command);
        return Ok(orderId);
    }
        
    [HttpGet("{id}")]
    public async Task<ActionResult<OrderDto>> GetOrder(Guid id)
    {
        var order = await _mediator.Send(new GetOrderQuery(id));
        return Ok(order);
    }
}