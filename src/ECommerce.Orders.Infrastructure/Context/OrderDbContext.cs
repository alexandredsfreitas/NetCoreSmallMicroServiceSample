using ECommerce.Orders.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Orders.Infrastructure.Context;

public class OrderDbContext: DbContext
{
    public DbSet<Order> Orders { get; set; }
        
    public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options) { }
        
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id)
                .HasConversion(
                    orderId => orderId.Value,
                    value => new OrderId(value));
                        
            entity.Property(e => e.ProductId)
                .HasConversion(
                    productId => productId.Value,
                    value => new ProductId(value));
                        
            entity.Property(e => e.Status)
                .HasConversion<string>();
                    
            entity.Ignore(e => e.Events);
        });
    }
}