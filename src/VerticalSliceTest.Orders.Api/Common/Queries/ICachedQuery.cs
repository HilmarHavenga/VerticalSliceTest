namespace VerticalSliceTest.Orders.Api.Common.Queries;

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
