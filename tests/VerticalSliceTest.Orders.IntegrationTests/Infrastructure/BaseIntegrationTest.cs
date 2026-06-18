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

    protected IRequestHandler<TRequest, TResponse> Handler<TRequest, TResponse>()
        where TRequest : notnull
    {
        return Scope.ServiceProvider.GetRequiredService<IRequestHandler<TRequest, TResponse>>();
    }
}
