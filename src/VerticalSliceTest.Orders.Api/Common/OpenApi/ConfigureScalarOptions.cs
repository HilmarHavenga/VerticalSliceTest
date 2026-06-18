namespace VerticalSliceTest.Orders.Api.Common.OpenApi;

internal sealed class ConfigureScalarOptions : IConfigureNamedOptions<ScalarOptions>
{
    private readonly IApiVersionDescriptionProvider _provider;

    public ConfigureScalarOptions(IApiVersionDescriptionProvider provider)
    {
        _provider = provider;
    }

    public void Configure(string? name, ScalarOptions options)
    {
        Configure(options);
    }

    public void Configure(ScalarOptions options)
    {
        foreach (ApiVersionDescription description in _provider.ApiVersionDescriptions)
        {
            options.AddDocument(
                description.GroupName,
                $"VerticalSliceTest.Orders.Api v{description.ApiVersion}",
                $"/openapi/{description.GroupName}.json");
        }
    }
}