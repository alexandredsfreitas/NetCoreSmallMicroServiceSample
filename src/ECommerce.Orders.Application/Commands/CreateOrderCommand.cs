using MediatR;

namespace ECommerce.Orders.Application.Commands;

public record CreateOrderCommand(Guid ProductId, int Quantity) : IRequest<Guid>;