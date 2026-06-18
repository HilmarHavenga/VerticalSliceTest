namespace VerticalSliceTest.Orders.Api.Infrastructure.Persistence.Outbox;

internal sealed class OutboxOptions
{
    public int IntervalInSeconds
    {
        get; init;
    }

    public int BatchSize
    {
        get; init;
    }
}