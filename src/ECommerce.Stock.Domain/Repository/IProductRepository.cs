using ECommerce.Stock.Domain.Models;

namespace ECommerce.Stock.Domain.Repository;

public interface IProductRepository
{
    Task<Product?> GetByIdAsync(ProductId id);
    Task AddAsync(Product product);
    Task UpdateAsync(Product product);
}