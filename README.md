# Vertical Slice Orders API

This is a vertical-slice ASP.NET Core API template for an Orders microservice. It demonstrates feature-based organization, custom request pipelines, EF Core persistence, Result-based handlers, RabbitMQ integration events, outbox/inbox reliability patterns, health checks, and integration-first testing.

## Architecture

The project is organized by feature first, with shared application plumbing in `Common` and external adapters in `Infrastructure`.

```text
src/VerticalSliceTest.Orders.Api/
  Common/
  Features/
  Infrastructure/
  Program.cs
```

## Patterns Used

- **Vertical slice architecture:** each use case owns its endpoint, request/query/command, handler, validator, and response.
- **Minimal APIs:** endpoints are discovered by scanning classes implementing `IEndpoints`.
- **Custom command/query pipeline:** commands use `ICommandHandler<TCommand,TResponse>` and queries use `IQueryHandler<TQuery,TResponse>`.
- **Scrutor scanning:** request handlers, validators, and integration event handlers are registered by assembly scan.
- **Result pattern:** handlers return `Result<T>` for expected success/failure outcomes.
- **Global exception handling:** unexpected and pipeline exceptions are converted to clean `ProblemDetails` responses.
- **EF Core with SQL Server:** persistence is wired through `ApplicationDbContext`.
- **Outbox pattern:** feature handlers save outgoing integration events in the database with business changes.
- **Inbox pattern:** inbound integration events are stored and deduplicated before handler dispatch.
- **RabbitMQ pub/sub:** RabbitMQ is the current broker behind the messaging abstractions.
- **Dead-letter queue:** failed inbound RabbitMQ messages are routed to a DLQ instead of being lost or retried forever.
- **Distributed cache abstraction:** handlers use `ICacheService`; the backing store can be swapped.
- **Integration-first testing:** integration and functional tests are preferred; unit tests are reserved for difficult edge cases.

## Runtime Flow

### HTTP Command Flow

```text
Endpoint
-> ICommandHandler<TCommand, Result<TResponse>>
-> Logging decorator
-> Validation decorator
-> Unit of work decorator
-> Handler
-> EF Core
-> Result<TResponse>
-> HTTP response
```

### Outbox Flow

```text
Handler adds business state
-> Handler adds OutboxMessage
-> Unit of work decorator commits both
-> ProcessOutboxMessagesService polls pending rows
-> OutboxMessageProcessor publishes through IPublisher
-> RabbitMqPublisher publishes IntegrationEventEnvelope
```

### Inbox Flow

```text
RabbitMQ message
-> RabbitMqConsumerService
-> deserialize IntegrationEventEnvelope
-> InboxMessageProcessor
-> check InboxMessages for duplicate
-> dispatch IIntegrationEventHandler<TEvent>
-> mark processed or failed
```

## Creating a Feature

Create a folder under `Features/<Area>/<UseCase>`.

Example:

```text
Features/
  Orders/
    CreateOrder/
      CreateOrderEndpoint.cs
      CreateOrderRequest.cs
      CreateOrderResponse.cs
      CreateOrderValidator.cs
      CreateOrderHandler.cs
```

### 1. Request

Use a request class or record for client input.

```csharp
public sealed class CreateOrderRequest : ICommand<Result<CreateOrderResponse>>
{
    public string CustomerName { get; init; } = string.Empty;
    public decimal TotalAmount { get; init; }
}
```

Do not include server-owned fields like `CreatedOnUtc`, `Status`, or internal IDs unless the client truly controls them.

### 2. Response

Return a response DTO from the handler.

```csharp
public sealed record CreateOrderResponse(
    Guid Id,
    string CustomerName,
    decimal TotalAmount,
    OrderStatus Status,
    DateTime CreatedOnUtc);
```

### 3. Validator

Implement `IRequestValidator<TRequest>`.

```csharp
internal sealed class CreateOrderValidator : IRequestValidator<CreateOrderRequest>
{
    public IEnumerable<RequestValidationFailure> Validate(CreateOrderRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.CustomerName))
        {
            yield return new RequestValidationFailure(nameof(request.CustomerName), "Customer name is required.");
        }
    }
}
```

Validators are discovered automatically by Scrutor.

### 4. Handler

Implement `ICommandHandler<TCommand, Result<TResponse>>` for commands or `IQueryHandler<TQuery, Result<TResponse>>` for queries.

```csharp
internal sealed class CreateOrderHandler : ICommandHandler<CreateOrderRequest, Result<CreateOrderResponse>>
{
    public async Task<Result<CreateOrderResponse>> Handle(CreateOrderRequest request, CancellationToken cancellationToken)
    {
        // create domain/persistence model
        // add database state
        // optionally write outbox message
        // do not call SaveChangesAsync for commands; the UoW decorator commits
        // return Result.Success(response)
    }
}
```

Handlers are discovered automatically by Scrutor.

### 5. Endpoint

Implement `IEndpoints`.

```csharp
internal sealed class CreateOrderEndpoint : IEndpoints
{
    public static void DefineEndpoints(IVersionedEndpointRouteBuilder app)
    {
        RouteGroupBuilder versioned = app.MapGroup("/api/v{version:apiVersion}/orders")
            .AllowAnonymous()
            .HasApiVersion(Versions.V1)
            .ReportApiVersions();

        versioned.MapPost("/", CreateOrderAsync)
            .HasApiVersion(Versions.V1)
            .WithName("CreateOrder");
    }

    private static async Task<IResult> CreateOrderAsync(
        CreateOrderRequest request,
        ICommandHandler<CreateOrderRequest, Result<CreateOrderResponse>> handler,
        CancellationToken cancellationToken)
    {
        Result<CreateOrderResponse> result = await handler.Handle(request, cancellationToken);

        return result.IsSuccess
            ? Results.Created($"/api/v1/orders/{result.Value.Id}", result.Value)
            : result.Error.ToFailureResult("Failed to create order");
    }
}
```

Endpoints are discovered automatically by `app.UseEndpoints<IApiMarker>()`.

## Adding Persistence

Put shared feature persistence types under `Features/<Area>/Common`.

Example:

```text
Features/
  Orders/
    Common/
      Order.cs
      OrderStatus.cs
      OrderConfiguration.cs
```

Add a `DbSet<T>` to `ApplicationDbContext` when the aggregate needs direct access:

```csharp
public DbSet<Order> Orders => Set<Order>();
```

Add EF configuration with `IEntityTypeConfiguration<T>`. Configurations are discovered by:

```csharp
modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
```

Create migrations under:

```text
Common/Database/Migrations
```

Command:

```powershell
dotnet ef migrations add InitialCreate `
  --project .\src\VerticalSliceTest.Orders.Api\VerticalSliceTest.Orders.Api.csproj `
  --startup-project .\src\VerticalSliceTest.Orders.Api\VerticalSliceTest.Orders.Api.csproj `
  --output-dir Common\Database\Migrations
```

In development, migrations are applied at startup by `ApplyMigrationsAsync()`.

## Publishing Integration Events

Use integration events for cross-service communication.

```text
Features/
  Orders/
    Events/
      OrderCreatedIntegrationEvent.cs
```

Integration events implement `IIntegrationEvent`.

```csharp
internal sealed record OrderCreatedIntegrationEvent(
    Guid Id,
    DateTime OccurredOnUtc,
    Guid OrderId) : IIntegrationEvent;
```

For reliable publishing, write an `OutboxMessage` in the same transaction as the business state.

The outbox processor later publishes through `IPublisher`.

## Consuming Integration Events

Add an event handler:

```csharp
internal sealed class SomeEventHandler : IIntegrationEventHandler<SomeIntegrationEvent>
{
    public Task HandleAsync(SomeIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
```

Handlers are discovered automatically by Scrutor. RabbitMQ messages are consumed by `RabbitMqConsumerService`, deduplicated through the inbox, and dispatched to matching handlers.

## Caching Queries

Make a query implement `IQuery<TResponse>` and `ICachedQuery`.

```csharp
internal sealed record GetOrderByIdQuery(Guid Id) : IQuery<Result<GetOrderByIdResponse>>, ICachedQuery
{
    public string CacheKey => $"orders:{Id}";
    public TimeSpan Expiration => TimeSpan.FromMinutes(5);
}
```

Only successful results are cached when the response exposes `IsSuccess`.

## Testing Guidance

Prefer integration and functional tests.

- **Integration tests:** use real DI, EF Core, Testcontainers, and handlers. Good for application behavior.
- **Functional tests:** use HTTP through the API host. Good for endpoint contracts, routing, auth, serialization, and ProblemDetails.
- **Unit tests:** reserve for edge-case behavior that is hard to reach through integration tests.

Integration test base exposes:

```csharp
protected ICommandHandler<TCommand, TResponse> CommandHandler<TCommand, TResponse>()
    where TCommand : ICommand<TResponse>

protected IQueryHandler<TQuery, TResponse> QueryHandler<TQuery, TResponse>()
    where TQuery : IQuery<TResponse>
```

Use `CommandHandler<TCommand,TResponse>()` or `QueryHandler<TQuery,TResponse>()` instead of MediatR `ISender`.

## Configuration

Main sections:

```json
{
  "ConnectionStrings": {
    "Database": ""
  },
  "Outbox": {
    "IntervalInSeconds": 10,
    "BatchSize": 20
  },
  "RabbitMq": {
    "HostName": "localhost",
    "Port": 5672,
    "UserName": "",
    "Password": "",
    "ExchangeName": "orders.integration-events",
    "QueueName": "orders-api.integration-events",
    "DeadLetterExchangeName": "orders.integration-events.dlx",
    "DeadLetterQueueName": "orders-api.integration-events.dlq",
    "PrefetchCount": 10,
    "RoutingKeys": [ "#" ]
  }
}
```

Do not commit real credentials. Use user secrets, environment variables, or untracked local settings for secrets.

## Clone Checklist

When cloning this template for a new service:

1. Rename project and namespaces.
2. Rename RabbitMQ exchange, queue, and DLQ names.
3. Change database name.
4. Replace Orders sample features with service-specific features.
5. Keep `Common` and `Infrastructure` patterns unless the service has a real reason to differ.
6. Add one complete create/read feature before adding more infrastructure.
