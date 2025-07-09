using ECommerce.Infrastructure.Messaging;
using ECommerce.SharedKernel.Events;
using ECommerce.Stock.Application.CommandHandlers;
using ECommerce.Stock.Application.EventHandlers;
using ECommerce.Stock.Domain.Repository;
using ECommerce.Stock.Infrastructure.Context;
using ECommerce.Stock.Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Stock.Api;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        // Controllers e MediatR
        services.AddControllers();
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(AddProductHandler).Assembly));
            
        // Database
        services.AddDbContext<StockDbContext>(options =>
            options.UseSqlServer("Server=localhost;Database=StockDB;Trusted_Connection=true;"));
            
        // Repositories
        services.AddScoped<IProductRepository, ProductRepository>();
            
        // Messaging
        services.AddSingleton<IEventBus, RabbitMQEventBus>();
            
        // Event Handlers
        services.AddScoped<OrderCreatedEventHandler>();
    }
        
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IEventBus eventBus)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
            
        app.UseRouting();
        app.UseEndpoints(endpoints => endpoints.MapControllers());
            
        // Configurar assinatura de eventos
        var serviceProvider = app.ApplicationServices;
        eventBus.Subscribe<OrderCreatedEvent>(async @event =>
        {
            using var scope = serviceProvider.CreateScope();
            var handler = scope.ServiceProvider.GetRequiredService<OrderCreatedEventHandler>();
            await handler.Handle(@event);
        });
    }
}