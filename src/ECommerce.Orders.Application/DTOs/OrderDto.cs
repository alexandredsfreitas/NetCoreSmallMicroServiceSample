namespace ECommerce.Orders.Application.DTOs;

public record OrderDto(Guid Id, Guid ProductId, int Quantity, string Status, DateTime CreatedAt);