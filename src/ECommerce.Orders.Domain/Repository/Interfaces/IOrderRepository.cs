using ECommerce.Orders.Domain.Models;

namespace ECommerce.Orders.Domain.Repository.Interfaces;

public interface IOrderRepository
{
    Task<Order?> GetByIdAsync(OrderId id);
    Task AddAsync(Order order);
    Task UpdateAsync(Order order);
}