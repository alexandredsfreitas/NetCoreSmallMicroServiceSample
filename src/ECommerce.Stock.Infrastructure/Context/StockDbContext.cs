using ECommerce.Stock.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Stock.Infrastructure.Context;

public class StockDbContext : DbContext
{
    public DbSet<Product> Products { get; set; }
        
    public StockDbContext(DbContextOptions<StockDbContext> options) : base(options) { }
        
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id)
                .HasConversion(
                    productId => productId.Value,
                    value => new ProductId(value));
                        
            entity.Property(e => e.AvailableQuantity)
                .HasConversion(
                    quantity => quantity.Value,
                    value => new StockQuantity(value));
                        
            entity.Property(e => e.ReservedQuantity)
                .HasConversion(
                    quantity => quantity.Value,
                    value => new StockQuantity(value));
                        
            entity.Ignore(e => e.Events);
        });
    }
}