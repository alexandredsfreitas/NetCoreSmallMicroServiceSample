using ECommerce.Orders.Application.DTOs;
using MediatR;

namespace ECommerce.Orders.Application.Queries;

public record GetOrderQuery(Guid OrderId) : IRequest<OrderDto>;