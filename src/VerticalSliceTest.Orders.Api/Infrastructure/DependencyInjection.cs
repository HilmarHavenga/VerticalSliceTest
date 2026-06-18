namespace VerticalSliceTest.Orders.Api.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration, nameof(configuration));

        services.AddTransient<IDateTimeProvider, DateTimeProvider>();

        services.AddCaching(configuration);
        services.AddPersistence(configuration);
        services.AddMessaging(configuration);
        AddHealthChecks(services, configuration);

        return services;
    }

    private static void AddHealthChecks(IServiceCollection services, IConfiguration configuration)
    {
        services.AddHealthChecks()
            .AddSqlServer(configuration.GetConnectionString("Database")!)
            .AddRabbitMQ(sp => sp.GetRequiredService<IConnection>());
    }
}
