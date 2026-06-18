namespace VerticalSliceTest.Orders.Api.Infrastructure.Messaging;

internal sealed class RabbitMqOptions
{
    public string HostName { get; init; } = "localhost";

    public int Port { get; init; } = 5672;

    public string UserName { get; init; } = "guest";

    public string Password { get; init; } = "guest";

    public string ExchangeName { get; init; } = "orders.integration-events";

    public string QueueName { get; init; } = "orders-api.integration-events";

    public string DeadLetterExchangeName { get; init; } = "orders.integration-events.dlx";

    public string DeadLetterQueueName { get; init; } = "orders-api.integration-events.dlq";

    public ushort PrefetchCount { get; init; } = 10;

    public string[] RoutingKeys { get; init; } = ["#"];
}
