using ECommerce.Orders.Domain.Models;
using ECommerce.Orders.Domain.Repository.Interfaces;
using ECommerce.Orders.Infrastructure.Context;

namespace ECommerce.Orders.Infrastructure.Repository;

public class OrderRepository : IOrderRepository
{
    private readonly OrderDbContext _context;
        
    public OrderRepository(OrderDbContext context)
    {
        _context = context;
    }
        
    public async Task<Order?> GetByIdAsync(OrderId id)
    {
        return await _context.Orders.FindAsync(id.Value);
    }
        
    public async Task AddAsync(Order order)
    {
        _context.Orders.Add(order);
        await _context.SaveChangesAsync();
    }
        
    public async Task UpdateAsync(Order order)
    {
        _context.Orders.Update(order);
        await _context.SaveChangesAsync();
    }
}