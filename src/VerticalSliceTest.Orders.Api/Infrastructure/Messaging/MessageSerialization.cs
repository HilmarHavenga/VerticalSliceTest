namespace VerticalSliceTest.Orders.Api.Infrastructure.Messaging;

internal static class MessageSerialization
{
    internal static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);

    public static string Serialize<T>(T message)
    {
        return JsonSerializer.Serialize(message, SerializerOptions);
    }

    public static string Serialize(object message, Type messageType)
    {
        return JsonSerializer.Serialize(message, messageType, SerializerOptions);
    }

    public static T? Deserialize<T>(string payload)
    {
        return JsonSerializer.Deserialize<T>(payload, SerializerOptions);
    }

    public static object? Deserialize(string payload, Type returnType)
    {
        return JsonSerializer.Deserialize(payload, returnType, SerializerOptions);
    }
}
