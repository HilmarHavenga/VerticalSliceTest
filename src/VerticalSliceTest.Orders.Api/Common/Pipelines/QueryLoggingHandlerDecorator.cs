namespace VerticalSliceTest.Orders.Api.Common.Pipelines;

internal sealed class QueryLoggingHandlerDecorator<TQuery, TResponse>(
    IQueryHandler<TQuery, TResponse> inner,
    ILogger<QueryLoggingHandlerDecorator<TQuery, TResponse>> logger)
    : IQueryHandler<TQuery, TResponse>
    where TQuery : IQuery<TResponse>
{
    public async Task<TResponse> Handle(TQuery query, CancellationToken cancellationToken)
    {
        string queryName = typeof(TQuery).Name;
        QueryHandlerLog.Handling(logger, queryName);

        try
        {
            return await inner.Handle(query, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception exception)
        {
            QueryHandlerLog.Failed(logger, exception, queryName);
            throw;
        }
        finally
        {
            QueryHandlerLog.Handled(logger, queryName);
        }
    }
}
