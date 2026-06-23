var builder = WebApplication.CreateBuilder(args);

builder.AddTelemetry();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();
builder.Services.AddRequestHandlersFromAssembly(typeof(IApiMarker).Assembly);
builder.Services.AddEndpointVersioning();
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();
builder.Services.ConfigureOptions<ConfigureScalarOptions>();

var app = builder.Build();

app.UseExceptionHandler();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapHealthChecks("/health");
app.UseEndpoints<IApiMarker>();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();

    await app.ApplyMigrationsAsync();
    //app.SeedData();
}

app.Run();
