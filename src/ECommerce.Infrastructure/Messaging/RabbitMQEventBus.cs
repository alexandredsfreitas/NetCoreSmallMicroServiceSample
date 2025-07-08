using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace ECommerce.Infrastructure.Messaging;

public class RabbitMQEventBus : IEventBus, IDisposable
{
    private readonly IConnection _connection;
    private readonly IChannel _channel;
    private readonly ILogger<RabbitMQEventBus> _logger;

    private RabbitMQEventBus(IConnection connection, IChannel channel, ILogger<RabbitMQEventBus> logger)
    {
        _connection = connection;
        _channel = channel;
        _logger = logger;
    }

    public static async Task<RabbitMQEventBus> CreateAsync(ILogger<RabbitMQEventBus> logger)
    {
        var factory = new ConnectionFactory()
        {
            HostName = "localhost",
            UserName = "guest",
            Password = "guest"
        };

        var connection = await factory.CreateConnectionAsync();
        var channel = await connection.CreateChannelAsync();

        // Declarar exchange
        await channel.ExchangeDeclareAsync(
            exchange: "ecommerce.events", 
            type: ExchangeType.Topic,
            durable: true,
            autoDelete: false);

        return new RabbitMQEventBus(connection, channel, logger);
    }

    public async Task PublishAsync<T>(T @event) where T : class
    {
        var routingKey = @event.GetType().Name;
        var message = JsonSerializer.Serialize(@event);
        var body = Encoding.UTF8.GetBytes(message);

        var properties = new BasicProperties
        {
            DeliveryMode = DeliveryModes.Persistent, // Torna a mensagem persistente
            Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds())
        };

        await _channel.BasicPublishAsync(
            exchange: "ecommerce.events",
            routingKey: routingKey,
            mandatory: false,
            basicProperties: properties,
            body: body);

        _logger.LogInformation("Evento {EventType} publicado: {Event}", typeof(T).Name, message);
    }

    public void Subscribe<T>(Func<T, Task> handler) where T : class
    {
        SubscribeAsync(handler).GetAwaiter().GetResult();
    }

    public async Task SubscribeAsync<T>(Func<T, Task> handler) where T : class
    {
        var queueName = $"{typeof(T).Name}.{Environment.MachineName}";
        var routingKey = typeof(T).Name;

        await _channel.QueueDeclareAsync(
            queue: queueName, 
            durable: true, 
            exclusive: false, 
            autoDelete: false);

        await _channel.QueueBindAsync(
            queue: queueName, 
            exchange: "ecommerce.events", 
            routingKey: routingKey);

        var consumer = new AsyncEventingBasicConsumer(_channel);
        
        consumer.ReceivedAsync += async (model, ea) =>
        {
            try
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var @event = JsonSerializer.Deserialize<T>(message);
                
                if (@event != null)
                {
                    await handler(@event);
                    await _channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);
                    _logger.LogInformation("Evento {EventType} processado com sucesso", typeof(T).Name);
                }
                else
                {
                    _logger.LogWarning("Evento {EventType} deserializado como null", typeof(T).Name);
                    await _channel.BasicNackAsync(deliveryTag: ea.DeliveryTag, multiple: false, requeue: false);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar evento {EventType}", typeof(T).Name);
                await _channel.BasicNackAsync(deliveryTag: ea.DeliveryTag, multiple: false, requeue: true);
            }
        };

        await _channel.BasicConsumeAsync(
            queue: queueName, 
            autoAck: false, 
            consumer: consumer);
    }

    public void Dispose()
    {
        DisposeAsync().AsTask().GetAwaiter().GetResult();
    }

    public async ValueTask DisposeAsync()
    {
        if (_channel != null)
            await _channel.CloseAsync();
        
        if (_connection != null)
            await _connection.CloseAsync();
    }
}