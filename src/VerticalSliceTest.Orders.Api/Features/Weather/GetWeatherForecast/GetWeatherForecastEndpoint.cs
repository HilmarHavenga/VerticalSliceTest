namespace VerticalSliceTest.Orders.Api.Features.Weather.GetWeatherForecast;

internal sealed class GetWeatherForecastEndpoint : IEndpoints
{
    public static void DefineEndpoints(IVersionedEndpointRouteBuilder app)
    {
        RouteGroupBuilder versioned = app.MapGroup("/api/v{version:apiVersion}/weather")
           //.RequireAuthorization()
           .AllowAnonymous()
           .HasApiVersion(Versions.V1)
           .ReportApiVersions();

        versioned.MapGet("/weather-forecast", GetWeatherForecastAsync)
           .HasApiVersion(Versions.V1)
           .WithName("GetWeatherForecast");
    }

    internal static async Task<IResult> GetWeatherForecastAsync(
        IQueryHandler<GetWeatherForecastQuery, Result<GetWeatherForecastResponse[]>> handler,
        CancellationToken cancellationToken)
    {
        Result<GetWeatherForecastResponse[]> result = await handler
            .Handle(new GetWeatherForecastQuery(), cancellationToken)
            .ConfigureAwait(false);

        return result.IsSuccess
            ? Results.Ok(result.Value)
            : result.Error.ToFailureResult("Failed to get weather forecast");
    }
}
