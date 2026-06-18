namespace VerticalSliceTest.Orders.Api.Features.Weather.GetWeatherForecast;

internal sealed record GetWeatherForecastResponse(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
