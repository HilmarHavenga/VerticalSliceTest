namespace VerticalSliceTest.Orders.FunctionalTests.Infrastructure;

public abstract class BaseFunctionalTest : IClassFixture<FunctionalTestWebAppFactory>
{
    private static readonly MethodInfo _dbContextSetMethod = typeof(DbContext)
        .GetMethods()
        .Single(methodInfo => methodInfo is { Name: nameof(DbContext.Set), IsGenericMethod: true } && methodInfo.GetParameters().Length == 0);

    protected BaseFunctionalTest(FunctionalTestWebAppFactory factory)
    {
        ArgumentNullException.ThrowIfNull(factory, nameof(factory));

        Factory = factory;
        HttpClient = factory.CreateClient();
    }

    protected HttpClient HttpClient
    {
        get;
    }

    protected FunctionalTestWebAppFactory Factory
    {
        get;
    }
}
