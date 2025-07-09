using MediatR;

namespace ECommerce.Stock.Application.Commands;

public record AddStockCommand(Guid ProductId, int Quantity) : IRequest;