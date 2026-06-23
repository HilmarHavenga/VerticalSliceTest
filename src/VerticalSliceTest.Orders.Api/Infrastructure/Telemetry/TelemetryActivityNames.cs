namespace VerticalSliceTest.Orders.Api.Infrastructure.Telemetry;

internal static class TelemetryActivityNames
{
    public const string OutboxProcess = "outbox process";

    public const string OutboxPublishMessage = "outbox publish message";

    public const string InboxProcess = "inbox process";

    public static string Command(string commandName) => $"command {commandName}";

    public static string Query(string queryName) => $"query {queryName}";

    public static string MessagingPublish(string eventName) => $"messaging publish {eventName}";

    public static string MessagingConsume(string eventName) => $"messaging consume {eventName}";
}
