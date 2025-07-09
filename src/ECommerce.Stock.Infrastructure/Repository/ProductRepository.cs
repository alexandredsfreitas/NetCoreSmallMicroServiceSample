using ECommerce.Stock.Domain.Models;
using ECommerce.Stock.Domain.Repository;
using ECommerce.Stock.Infrastructure.Context;

namespace ECommerce.Stock.Infrastructure.Repository;

public class ProductRepository : IProductRepository
{
    private readonly StockDbContext _context;
        
    public ProductRepository(StockDbContext context)
    {
        _context = context;
    }
        
    public async Task<Product?> GetByIdAsync(ProductId id)
    {
        return await _context.Products.FindAsync(id.Value);
    }
        
    public async Task AddAsync(Product product)
    {
        _context.Products.Add(product);
        await _context.SaveChangesAsync();
    }
        
    public async Task UpdateAsync(Product product)
    {
        _context.Products.Update(product);
        await _context.SaveChangesAsync();
    }
}