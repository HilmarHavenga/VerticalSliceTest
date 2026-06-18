namespace VerticalSliceTest.Orders.Api.Infrastructure.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        string connectionString = configuration.GetConnectionString("Database") ??
            throw new InvalidOperationException("Connection string 'Database' was not found.");

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseSqlServer(connectionString);
        });

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<ApplicationDbContext>());
        services.AddScoped<IInboxMessageProcessor, InboxMessageProcessor>();
        services.AddScoped<IOutboxMessageProcessor, OutboxMessageProcessor>();
        AddOutbox(services, configuration);

        return services;
    }

    private static void AddOutbox(IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<OutboxOptions>(configuration.GetSection("Outbox"));
        services.AddHostedService<ProcessOutboxMessagesService>();
    }
}
