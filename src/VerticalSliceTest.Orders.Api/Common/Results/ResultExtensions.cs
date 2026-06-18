namespace VerticalSliceTest.Orders.Api.Common.Results;

using Microsoft.AspNetCore.Http;

internal static class ResultExtensions
{
    public static IResult ToFailureResult(
        this Failure failure,
        string problemTitle,
        IReadOnlyDictionary<string, int>? statusCodeByFailureCode = null,
        int defaultStatusCode = StatusCodes.Status400BadRequest)
    {
        int statusCode = defaultStatusCode;
        if (statusCodeByFailureCode is not null && statusCodeByFailureCode.TryGetValue(failure.Code, out int mappedStatusCode))
        {
            statusCode = mappedStatusCode;
        }

        return Results.Problem(
            statusCode: statusCode,
            title: problemTitle,
            type: failure.Code,
            detail: failure.Description);
    }
}