namespace VerticalSliceTest.Orders.Api.Common.Pipelines;

public static class PipelineExtensions
{
    public static IServiceCollection AddRequestHandlersFromAssembly(
        this IServiceCollection services,
        Assembly assembly)
    {
        services.Scan(scan => scan
            .FromAssemblies(assembly)
            .AddClasses(classes => classes.AssignableTo(typeof(IRequestHandler<,>)), publicOnly: false)
            .AsImplementedInterfaces()
            .WithTransientLifetime());

        services.Scan(scan => scan
            .FromAssemblies(assembly)
            .AddClasses(classes => classes.AssignableTo(typeof(IRequestValidator<>)), publicOnly: false)
            .AsImplementedInterfaces()
            .WithTransientLifetime());

        services.Scan(scan => scan
            .FromAssemblies(assembly)
            .AddClasses(classes => classes.AssignableTo(typeof(IIntegrationEventHandler<>)), publicOnly: false)
            .AsImplementedInterfaces()
            .WithTransientLifetime());

        services.Decorate(typeof(IRequestHandler<,>), typeof(ValidationHandlerDecorator<,>));
        services.Decorate(typeof(IRequestHandler<,>), typeof(QueryCachingHandlerDecorator<,>));
        services.Decorate(typeof(IRequestHandler<,>), typeof(LoggingHandlerDecorator<,>));

        return services;
    }
}
