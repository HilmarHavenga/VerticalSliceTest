namespace VerticalSliceTest.Orders.Api.Common.Errors;

internal sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        ExceptionDetails details = GetExceptionDetails(exception);

        if (details.Status == StatusCodes.Status500InternalServerError)
        {
            logger.LogError(exception, "Unhandled exception occurred.");
        }
        else
        {
            logger.LogWarning(exception, "Handled exception occurred.");
        }

        httpContext.Response.StatusCode = details.Status;

        if (exception is RequestValidationException validationException)
        {
            await Microsoft.AspNetCore.Http.Results.ValidationProblem(
                validationException.Errors,
                statusCode: details.Status,
                title: details.Title,
                type: details.Type,
                detail: details.Detail)
                .ExecuteAsync(httpContext);

            return true;
        }

        ProblemDetails problemDetails = new()
        {
            Status = details.Status,
            Type = details.Type,
            Title = details.Title,
            Detail = details.Detail,
        };

        await Microsoft.AspNetCore.Http.Results.Problem(problemDetails).ExecuteAsync(httpContext);

        return true;
    }

    private static ExceptionDetails GetExceptionDetails(Exception exception) => exception switch
    {
        RequestValidationException => new ExceptionDetails(
            StatusCodes.Status400BadRequest,
            "ValidationFailure",
            "Validation error",
            "One or more validation errors occurred."),

        ConcurrencyException => new ExceptionDetails(
            StatusCodes.Status409Conflict,
            "ConcurrencyConflict",
            "Concurrency conflict",
            "The record was modified by another process."),

        _ => new ExceptionDetails(
            StatusCodes.Status500InternalServerError,
            "ServerError",
            "Server error",
            "An unexpected error occurred."),
    };

    private sealed record ExceptionDetails(
        int Status,
        string Type,
        string Title,
        string Detail);
}
