using ECommerce.Stock.Application.Commands;
using ECommerce.Stock.Domain.Models;
using ECommerce.Stock.Domain.Repository;
using MediatR;

namespace ECommerce.Stock.Application.CommandHandlers;

public class AddProductHandler : IRequestHandler<AddProductCommand>
{
    private readonly IProductRepository _repository;
        
    public AddProductHandler(IProductRepository repository)
    {
        _repository = repository;
    }
        
    public async Task Handle(AddProductCommand request, CancellationToken cancellationToken)
    {
        var product = new Product(
            new ProductId(request.ProductId), 
            request.Name, 
            new StockQuantity(request.InitialQuantity));
                
        await _repository.AddAsync(product);
    }
}