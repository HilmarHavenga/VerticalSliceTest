namespace VerticalSliceTest.Orders.Api.Common.Queries;

internal sealed class QueryTelemetryHandlerDecorator<TQuery, TResponse>(
    IQueryHandler<TQuery, TResponse> inner)
    : IQueryHandler<TQuery, TResponse>
    where TQuery : IQuery<TResponse>
{
    public async Task<TResponse> Handle(TQuery query, CancellationToken cancellationToken)
    {
        string queryName = typeof(TQuery).Name;

        using Activity? activity = TelemetryActivitySource.StartActivity(TelemetryActivityNames.Query(queryName));
        activity?.SetTag(TelemetryTags.CodeFunction, queryName);
        activity?.SetTag(TelemetryTags.RequestType, TelemetryTagValues.Query);

        try
        {
            TResponse response = await inner.Handle(query, cancellationToken).ConfigureAwait(false);

            if (response is Result result)
            {
                activity?.SetTag(TelemetryTags.ResultSuccess, result.IsSuccess);
                activity?.SetTag(TelemetryTags.ResultFailureCode, result.Error.Code);
            }

            return response;
        }
        catch (Exception exception)
        {
            activity?.SetStatus(ActivityStatusCode.Error, exception.Message);
            activity?.AddException(exception);
            throw;
        }
    }
}
