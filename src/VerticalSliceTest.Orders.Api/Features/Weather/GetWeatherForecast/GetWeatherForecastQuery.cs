namespace VerticalSliceTest.Orders.Api.Features.Weather.GetWeatherForecast;

internal sealed record GetWeatherForecastQuery : ICachedQuery
{
    public string CacheKey => "weather-forecast";

    public TimeSpan Expiration => TimeSpan.FromSeconds(30);
}
