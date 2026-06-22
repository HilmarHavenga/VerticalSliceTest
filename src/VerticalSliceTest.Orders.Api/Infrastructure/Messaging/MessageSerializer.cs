namespace VerticalSliceTest.Orders.Api.Infrastructure.Messaging;

internal sealed class MessageSerializer : IMessageSerializer
{
    private static readonly JsonSerializerOptions _serializerOptions = new(JsonSerializerDefaults.Web);

    public T? Deserialize<T>(string payload)
    {
        return JsonSerializer.Deserialize<T>(payload, _serializerOptions);
    }

    public object? Deserialize(string payload, Type returnType)
    {
        return JsonSerializer.Deserialize(payload, returnType, _serializerOptions);
    }

    public string Serialize<T>(T message)
    {
        return JsonSerializer.Serialize(message, _serializerOptions);
    }
}
