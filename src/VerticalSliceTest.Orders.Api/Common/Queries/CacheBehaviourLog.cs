namespace VerticalSliceTest.Orders.Api.Common.Pipelines;

internal static partial class CacheBehaviourLog
{
    [LoggerMessage(
        EventId = 4000,
        Level = LogLevel.Information,
        Message = "Cache hit for {QueryName}")]
    public static partial void CacheHit(ILogger logger, string queryName);

    [LoggerMessage(
        EventId = 4001,
        Level = LogLevel.Information,
        Message = "Cache miss for {QueryName}")]
    public static partial void CacheMiss(ILogger logger, string queryName);

    [LoggerMessage(
        EventId = 4002,
        Level = LogLevel.Information,
        Message = "Cache default for {QueryName}. Continuing without cache.")]
    public static partial void CacheDefault(ILogger logger, string queryName);
}
