using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

public class RabbitMQService
{
    private readonly string _hostname = "localhost";

    public async Task SendMessageAsync<T>(T message, string queueName)
    {
        var factory = new ConnectionFactory() { HostName = _hostname };

        using var connection = await factory.CreateConnectionAsync();
        using var channel = await connection.CreateChannelAsync();

        await channel.QueueDeclareAsync(queue: queueName,
                                        durable: false,
                                        exclusive: false,
                                        autoDelete: false,
                                        arguments: null);

        var json = JsonSerializer.Serialize(message);
        var body = Encoding.UTF8.GetBytes(json);

        await channel.BasicPublishAsync(exchange: string.Empty,
                                        routingKey: queueName,
                                        body: body);
    }
}