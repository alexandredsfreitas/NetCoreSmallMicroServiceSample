using ECommerce.Stock.Application.DTOs;
using MediatR;

namespace ECommerce.Stock.Application.Queries;

public record GetProductStockQuery(Guid ProductId) : IRequest<ProductStockDto>;