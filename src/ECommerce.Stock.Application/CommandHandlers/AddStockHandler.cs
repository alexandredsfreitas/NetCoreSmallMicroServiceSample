using ECommerce.Exceptions;
using ECommerce.Stock.Application.Commands;
using ECommerce.Stock.Domain.Models;
using ECommerce.Stock.Domain.Repository;
using MediatR;

namespace ECommerce.Stock.Application.CommandHandlers;

public class AddStockHandler : IRequestHandler<AddStockCommand>
{
    private readonly IProductRepository _repository;
        
    public AddStockHandler(IProductRepository repository)
    {
        _repository = repository;
    }
        
    public async Task Handle(AddStockCommand request, CancellationToken cancellationToken)
    {
        var product = await _repository.GetByIdAsync(new ProductId(request.ProductId));
        if (product == null)
            throw new NotFoundException($"Produto {request.ProductId} não encontrado");
                
        product.AddStock(request.Quantity);
        await _repository.UpdateAsync(product);
    }
}