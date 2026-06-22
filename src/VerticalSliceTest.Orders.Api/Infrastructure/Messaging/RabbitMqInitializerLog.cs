namespace VerticalSliceTest.Orders.Api.Infrastructure.Messaging;

internal static partial class RabbitMqInitializerLog
{
    [LoggerMessage(EventId = 3200, Level = LogLevel.Information, Message = "RabbitMQ exchange {ExchangeName} declared.")]
    public static partial void ExchangeDeclared(ILogger logger, string exchangeName);
}
