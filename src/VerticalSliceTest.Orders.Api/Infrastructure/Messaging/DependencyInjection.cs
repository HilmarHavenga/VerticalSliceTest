namespace VerticalSliceTest.Orders.Api.Infrastructure.Messaging;

public static class DependencyInjection
{
    public static IServiceCollection AddMessaging(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration, nameof(configuration));

        services.Configure<RabbitMqOptions>(configuration.GetSection("RabbitMq"));
        services.AddSingleton<IConnection>(sp =>
        {
            RabbitMqOptions options = sp.GetRequiredService<IOptions<RabbitMqOptions>>().Value;

            ConnectionFactory factory = new()
            {
                HostName = options.HostName,
                Port = options.Port,
                UserName = options.UserName,
                Password = options.Password
            };

            return factory.CreateConnectionAsync().GetAwaiter().GetResult();
        });

        services.AddSingleton<IPublisher, RabbitMqPublisher>();
        services.AddSingleton<IConsumer, RabbitMqConsumer>();
        services.AddHostedService<RabbitMqInitializer>();
        services.AddHostedService<IntegrationEventConsumerService>();

        return services;
    }
}
