using ECommerce.Stock.Application.Commands;
using ECommerce.Stock.Application.DTOs;
using ECommerce.Stock.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Stock.Api;

[ApiController]
[Route("api/[controller]")]
public class StockController : ControllerBase
{
    private readonly IMediator _mediator;
        
    public StockController(IMediator mediator)
    {
        _mediator = mediator;
    }
        
    [HttpPost("products")]
    public async Task<IActionResult> AddProduct([FromBody] AddProductCommand command)
    {
        await _mediator.Send(command);
        return Ok();
    }
        
    [HttpPost("products/{productId}/add-stock")]
    public async Task<IActionResult> AddStock(Guid productId, [FromBody] AddStockRequest request)
    {
        await _mediator.Send(new AddStockCommand(productId, request.Quantity));
        return Ok();
    }
        
    [HttpGet("products/{productId}")]
    public async Task<ActionResult<ProductStockDto>> GetProductStock(Guid productId)
    {
        var product = await _mediator.Send(new GetProductStockQuery(productId));
        return Ok(product);
    }
    
}

public record AddStockRequest(int Quantity);