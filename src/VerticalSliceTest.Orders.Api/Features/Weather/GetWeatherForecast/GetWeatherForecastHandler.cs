namespace VerticalSliceTest.Orders.Api.Features.Weather.GetWeatherForecast;

internal sealed class GetWeatherForecastHandler : IQueryHandler<GetWeatherForecastQuery, Result<GetWeatherForecastResponse[]>>
{
    private static readonly string[] Summaries =
    [
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    ];

    public Task<Result<GetWeatherForecastResponse[]>> Handle(GetWeatherForecastQuery request, CancellationToken cancellationToken)
    {
        GetWeatherForecastResponse[] forecast = [.. Enumerable.Range(1, 5).Select(index =>
            new GetWeatherForecastResponse
            (
                DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                Random.Shared.Next(-20, 55),
                Summaries[Random.Shared.Next(Summaries.Length)]
            ))];

        return Task.FromResult(Result.Success(forecast));
    }
}
