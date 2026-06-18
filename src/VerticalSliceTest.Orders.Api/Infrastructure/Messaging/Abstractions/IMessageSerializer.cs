namespace VerticalSliceTest.Orders.Api.Infrastructure.Messaging.Abstractions;

public interface IMessageSerializer
{
    string Serialize<T>(T message);

    T? Deserialize<T>(string payload);

    object? Deserialize(string payload, Type returnType);
}
