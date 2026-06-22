namespace VerticalSliceTest.Orders.Api.Common.Pipelines;

public static class PipelineExtensions
{
    public static IServiceCollection AddRequestHandlersFromAssembly(
        this IServiceCollection services,
        Assembly assembly)
    {
        services.Scan(scan => scan
            .FromAssemblies(assembly)
            .AddClasses(classes => classes.AssignableTo(typeof(ICommandHandler<,>)), publicOnly: false)
            .AsImplementedInterfaces()
            .WithTransientLifetime());

        services.Scan(scan => scan
            .FromAssemblies(assembly)
            .AddClasses(classes => classes.AssignableTo(typeof(IQueryHandler<,>)), publicOnly: false)
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

        services.Decorate(typeof(ICommandHandler<,>), typeof(UnitOfWorkCommandHandlerDecorator<,>));
        services.Decorate(typeof(ICommandHandler<,>), typeof(CommandValidationHandlerDecorator<,>));
        services.Decorate(typeof(ICommandHandler<,>), typeof(CommandLoggingHandlerDecorator<,>));

        services.Decorate(typeof(IQueryHandler<,>), typeof(QueryCachingHandlerDecorator<,>));
        services.Decorate(typeof(IQueryHandler<,>), typeof(QueryValidationHandlerDecorator<,>));
        services.Decorate(typeof(IQueryHandler<,>), typeof(QueryLoggingHandlerDecorator<,>));

        return services;
    }
}
