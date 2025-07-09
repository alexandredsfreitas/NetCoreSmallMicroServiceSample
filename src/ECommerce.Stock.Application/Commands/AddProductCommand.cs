using MediatR;

namespace ECommerce.Stock.Application.Commands;

public record AddProductCommand(Guid ProductId, string Name, int InitialQuantity) : IRequest;