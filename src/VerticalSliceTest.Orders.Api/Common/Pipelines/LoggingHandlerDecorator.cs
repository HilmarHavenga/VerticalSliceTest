namespace VerticalSliceTest.Orders.Api.Common.Pipelines;

internal sealed class LoggingHandlerDecorator<TRequest, TResponse>(
    IRequestHandler<TRequest, TResponse> inner,
    ILogger<LoggingHandlerDecorator<TRequest, TResponse>> logger)
    : IRequestHandler<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        logger.LogInformation("Handling {RequestName}", requestName);

        try
        {
            var response = await inner.Handle(request, cancellationToken);
            return response;
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Failed handling {RequestName}", requestName);
            throw;
        }
        finally
        {
            logger.LogInformation("Handled {RequestName}", requestName);
        }
    }
}
