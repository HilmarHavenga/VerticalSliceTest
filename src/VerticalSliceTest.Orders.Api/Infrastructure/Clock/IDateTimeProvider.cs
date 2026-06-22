namespace VerticalSliceTest.Orders.Api.Infrastructure.Clock;

public interface IDateTimeProvider
{
    DateTime UtcNow {  get; }
}