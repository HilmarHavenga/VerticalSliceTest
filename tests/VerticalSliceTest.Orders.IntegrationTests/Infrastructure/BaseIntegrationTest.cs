namespace VerticalSliceTest.Orders.IntegrationTests.Infrastructure;

public abstract class BaseIntegrationTest : IClassFixture<IntegrationTestWebAppFactory>
{
    protected BaseIntegrationTest(IntegrationTestWebAppFactory factory)
    {
        ArgumentNullException.ThrowIfNull(factory, nameof(factory));
        Scope = factory.Services.CreateScope();

        DbContext = Scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    }

    protected IServiceScope Scope
    {
        get;
    }

    protected ApplicationDbContext DbContext
    {
        get;
    }

    protected ICommandHandler<TCommand, TResponse> CommandHandler<TCommand, TResponse>()
        where TCommand : ICommand<TResponse>
    {
        return Scope.ServiceProvider.GetRequiredService<ICommandHandler<TCommand, TResponse>>();
    }

    protected IQueryHandler<TQuery, TResponse> QueryHandler<TQuery, TResponse>()
        where TQuery : IQuery<TResponse>
    {
        return Scope.ServiceProvider.GetRequiredService<IQueryHandler<TQuery, TResponse>>();
    }
}
