using ECommerce.Exceptions;
using ECommerce.Stock.Application.DTOs;
using ECommerce.Stock.Application.Queries;
using ECommerce.Stock.Domain.Models;
using ECommerce.Stock.Domain.Repository;
using MediatR;

namespace ECommerce.Stock.Application.QueryHandlers;

public class GetProductStockHandler : IRequestHandler<GetProductStockQuery, ProductStockDto>
{
    private readonly IProductRepository _repository;
        
    public GetProductStockHandler(IProductRepository repository)
    {
        _repository = repository;
    }
        
    public async Task<ProductStockDto> Handle(GetProductStockQuery request, CancellationToken cancellationToken)
    {
        var product = await _repository.GetByIdAsync(new ProductId(request.ProductId));
        if (product == null)
            throw new NotFoundException($"Produto {request.ProductId} não encontrado");
                
        return new ProductStockDto(
            product.Id.Value,
            product.Name,
            product.AvailableQuantity,
            product.ReservedQuantity);
    }
}