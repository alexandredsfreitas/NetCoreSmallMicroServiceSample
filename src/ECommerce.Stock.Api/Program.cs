using ECommerce.Infrastructure.Messaging;
using ECommerce.SharedKernel.Events;
using ECommerce.Stock.Application.CommandHandlers;
using ECommerce.Stock.Application.EventHandlers;
using ECommerce.Stock.Domain.Repository;
using ECommerce.Stock.Infrastructure.Context;
using ECommerce.Stock.Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Controllers e MediatR
builder.Services.AddControllers();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(AddProductHandler).Assembly));

// API Explorer e Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database
builder.Services.AddDbContext<StockDbContext>(options =>
    options.UseSqlServer("Server=localhost;Database=StockDB;Trusted_Connection=true;"));

// Repositories
builder.Services.AddScoped<IProductRepository, ProductRepository>();

// Messaging
builder.Services.AddSingleton<IEventBus, RabbitMQEventBus>();

// Event Handlers
builder.Services.AddScoped<OrderCreatedEventHandler>();

var app = builder.Build();

// Configure pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();
app.MapControllers();

// Configurar assinatura de eventos
var eventBus = app.Services.GetRequiredService<IEventBus>();
var serviceProvider = app.Services;

eventBus.Subscribe<OrderCreatedEvent>(async @event =>
{
    using var scope = serviceProvider.CreateScope();
    var handler = scope.ServiceProvider.GetRequiredService<OrderCreatedEventHandler>();
    await handler.Handle(@event);
});

app.Run();