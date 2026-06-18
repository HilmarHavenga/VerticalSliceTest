namespace VerticalSliceTest.Orders.FunctionalTests.Infrastructure;

public class FunctionalTestWebAppFactory : WebApplicationFactory<IApiMarker>, IAsyncLifetime
{
    private readonly MsSqlContainer _dbContainer = new MsSqlBuilder("mcr.microsoft.com/mssql/server:2022-latest")
        .Build();

    private readonly RedisContainer _valkeyContainer = new RedisBuilder("valkey/valkey:latest")
        .Build();

    private string _dbConnectionString = string.Empty;

    public async ValueTask InitializeAsync()
    {
        await _dbContainer.StartAsync();
        await _valkeyContainer.StartAsync();
        Microsoft.Data.SqlClient.SqlConnectionStringBuilder connectionStringBuilder = new(_dbContainer.GetConnectionString())
        {
            InitialCatalog = "VerticleSliceTest.Orders.Api",
        };
        _dbConnectionString = connectionStringBuilder.ConnectionString;

        using IServiceScope scope = Services.CreateScope();
        ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await dbContext.Database.EnsureDeletedAsync();
        await dbContext.Database.MigrateAsync();
    }

    public override async ValueTask DisposeAsync()
    {
        await _dbContainer.StopAsync();
        await _valkeyContainer.StopAsync();
        await base.DisposeAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<DbContextOptions<ApplicationDbContext>>();

            services.AddDbContext<ApplicationDbContext>(options =>
                options
                    .UseSqlServer(_dbConnectionString));

            services.Configure<RedisCacheOptions>(redisCacheOptions =>
                redisCacheOptions.Configuration = _valkeyContainer.GetConnectionString());
            //services.AddAuthentication(options =>
            //{
            //    options.DefaultAuthenticateScheme = TestAuthDefaults.Scheme;
            //    options.DefaultChallengeScheme = TestAuthDefaults.Scheme;
            //})
            //    .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(TestAuthDefaults.Scheme, _ => { });
        });
    }
}
