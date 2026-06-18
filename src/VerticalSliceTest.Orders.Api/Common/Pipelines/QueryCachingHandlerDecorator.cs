namespace VerticalSliceTest.Orders.Api.Common.Pipelines;

internal sealed class QueryCachingHandlerDecorator<TRequest, TResponse>(
    IRequestHandler<TRequest, TResponse> inner,
    ICacheService cacheService,
    ILogger<QueryCachingHandlerDecorator<TRequest, TResponse>> logger)
    : IRequestHandler<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken)
    {
        if (request is not ICachedQuery cachedQuery)
        {
            return await inner.Handle(request, cancellationToken).ConfigureAwait(false);
        }

        string requestName = typeof(TRequest).Name;
        TResponse? cachedResult = default;

        try
        {
            cachedResult = await cacheService.GetAsync<TResponse>(cachedQuery.CacheKey, cancellationToken).ConfigureAwait(false);
        }
        catch
        {
            CacheBehaviourLog.CacheDefault(logger, requestName);
        }

        if (cachedResult is not null)
        {
            CacheBehaviourLog.CacheHit(logger, requestName);

            return cachedResult;
        }

        CacheBehaviourLog.CacheMiss(logger, requestName);

        TResponse result = await inner.Handle(request, cancellationToken).ConfigureAwait(false);

        if (ShouldCache(result))
        {
            try
            {
                await cacheService.SetAsync(cachedQuery.CacheKey, result, cachedQuery.Expiration, cancellationToken).ConfigureAwait(false);
            }
            catch
            {
                CacheBehaviourLog.CacheDefault(logger, requestName);
            }
        }

        return result;
    }

    private static bool ShouldCache(TResponse result)
    {
        if (result is null)
        {
            return false;
        }

        Type responseType = result.GetType();
        PropertyInfo? isSuccessProperty = responseType.GetProperty("IsSuccess", BindingFlags.Instance | BindingFlags.Public);

        if (isSuccessProperty?.PropertyType == typeof(bool))
        {
            return (bool)(isSuccessProperty.GetValue(result) ?? false);
        }

        return true;
    }
}
