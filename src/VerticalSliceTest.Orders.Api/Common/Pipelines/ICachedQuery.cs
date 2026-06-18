namespace VerticalSliceTest.Orders.Api.Common.Pipelines;

public interface ICachedQuery
{
    string CacheKey
    {
        get;
    }

    TimeSpan Expiration
    {
        get;
    }
}
