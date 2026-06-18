namespace VerticalSliceTest.Orders.Api.Infrastructure.Persistence.Outbox;

internal sealed class OutboxMessage
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public OutboxMessage()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    {
    }

    public OutboxMessage(Guid id, DateTime occurredOnUtc, string type, string content)
    {
        Id = id;
        OccurredOnUtc = occurredOnUtc;
        Type = type;
        Content = content;
    }

    public Guid Id
    {
        get; init;
    }

    public DateTime OccurredOnUtc
    {
        get; init;
    }

    public string Type
    {
        get; init;
    }

    public string Content
    {
        get; init;
    }

    public DateTime? ProcessedOnUtc
    {
        get; private set;
    }

    public string? Error
    {
        get; private set;
    }

    internal void SetProcessedOnUtc(IDateTimeProvider dateTimeProvider)
    {
        ProcessedOnUtc = dateTimeProvider.UtcNow;
        Error = null;
    }

    internal void SetError(string error) => Error = error;
}
