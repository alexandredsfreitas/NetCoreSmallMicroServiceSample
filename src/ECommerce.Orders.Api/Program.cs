using ECommerce.Infrastructure.Messaging;
using ECommerce.Orders.Application.CommandHandlers;
using ECommerce.Orders.Application.EventHandlers;
using ECommerce.Orders.Domain.Repository.Interfaces;
using ECommerce.Orders.Infrastructure.Context;
using ECommerce.Orders.Infrastructure.Repository;
using ECommerce.SharedKernel.Events;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Controllers e MediatR
builder.Services.AddControllers();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateOrderHandler).Assembly));

// API Explorer e Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database
builder.Services.AddDbContext<OrderDbContext>(options =>
    options.UseSqlServer("Server=localhost;Database=OrdersDB;Trusted_Connection=true;"));

// Repositories
builder.Services.AddScoped<IOrderRepository, OrderRepository>();

// Messaging
builder.Services.AddSingleton<IEventBus, RabbitMQEventBus>();

// Event Handlers
builder.Services.AddScoped<StockReservedEventHandler>();
builder.Services.AddScoped<StockReservationFailedEventHandler>();

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

eventBus.Subscribe<StockReservedEvent>(async @event =>
{
    using var scope = serviceProvider.CreateScope();
    var handler = scope.ServiceProvider.GetRequiredService<StockReservedEventHandler>();
    await handler.Handle(@event);
});

eventBus.Subscribe<StockReservationFailedEvent>(async @event =>
{
    using var scope = serviceProvider.CreateScope();
    var handler = scope.ServiceProvider.GetRequiredService<StockReservationFailedEventHandler>();
    await handler.Handle(@event);
});

app.Run();