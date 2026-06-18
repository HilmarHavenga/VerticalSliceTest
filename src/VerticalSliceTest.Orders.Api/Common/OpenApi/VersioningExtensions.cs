namespace VerticalSliceTest.Orders.Api.Common.OpenApi;

public static class VersioningExtensions
{
    public static IServiceCollection AddEndpointVersioning(this IServiceCollection services)
    {
        foreach (string versionString in Versions.AllAsStrings)
        {
            services.AddOpenApi(versionString);
        }

        services.AddApiVersioning(
            options =>
            {
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;
                options.ApiVersionReader = new UrlSegmentApiVersionReader();
            })
            .AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });

        return services;
    }
}
