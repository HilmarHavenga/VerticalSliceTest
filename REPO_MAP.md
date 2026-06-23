# Repo Map

This repo is a vertical-slice ASP.NET Core API template for an Orders service. Use this file as orientation only. Read the actual source files before editing.

## Top Level

```text
src/
  VerticalSliceTest.Orders.Api/
tests/
  VerticalSliceTest.Orders.FunctionalTests/
  VerticalSliceTest.Orders.IntegrationTests/
  VerticalSliceTest.Orders.UnitTests/
```

The API project contains the application, infrastructure adapters, and feature slices. Tests are split by behavior level.

## API Structure

```text
src/VerticalSliceTest.Orders.Api/
  Common/
  Features/
  Infrastructure/
  Program.cs
```

- `Program.cs` is the composition root.
- `Common` contains shared application plumbing.
- `Features` contains vertical slices.
- `Infrastructure` contains external adapters such as EF Core, RabbitMQ, cache, clock, and telemetry.

## Common

Use `Common` for reusable application-level building blocks that are not owned by one feature and are not external-system implementations.

Current areas:

- `Database`: migration startup helpers and migrations.
- `Commands`: command contracts, command decorators, unit of work, command logs, command telemetry.
- `Queries`: query contracts, query decorators, cache behavior, query logs, query telemetry.
- `DependencyInjection`: handler registration and decorator wiring.
- `Endpoints`: endpoint discovery contracts/helpers.
- `Errors`: global exception handling.
- `OpenApi`: Scalar/OpenAPI/versioning setup.
- `Results`: Result pattern types and HTTP conversion helpers.
- `Validation`: request validators and validation exceptions.

## Features

Put new behavior under:

```text
Features/<Area>/<UseCase>/
```

Example:

```text
Features/Orders/CreateOrder/
```

A normal feature slice usually contains:

- Endpoint
- Request, command, or query
- Response DTO
- Validator
- Handler

Shared feature model/configuration goes under:

```text
Features/<Area>/Common/
```

Cross-service event contracts go under:

```text
Features/<Area>/Events/
```

## Infrastructure

Use `Infrastructure` only for implementations that talk to external systems or framework services.

Current areas:

- `Caching`: cache abstraction implementation.
- `Clock`: date/time provider implementation.
- `Messaging`: RabbitMQ, publisher/consumer abstractions, message serialization, topology, consumer.
- `Persistence`: EF Core, outbox, inbox, unit of work.
- `Telemetry`: OpenTelemetry options, exporter wiring, activity source, activity names, and tag constants.

Keep transport-specific logic here. Feature handlers should depend on abstractions, not RabbitMQ or SQL-specific details directly.

## Feature Workflow

When adding a feature:

1. Create a folder under `Features/<Area>/<UseCase>`.
2. Add the request/query/command and response DTO.
3. Add an `IRequestValidator<TRequest>` when input validation is needed.
4. Add an `ICommandHandler<TCommand, Result<TResponse>>` for writes or `IQueryHandler<TQuery, Result<TResponse>>` for reads.
5. Add an endpoint implementing the endpoint discovery pattern.
6. Add feature failures/problem titles instead of hardcoded failure strings.
7. Add or update EF configuration if persistence changes.
8. Add integration or functional tests first.
9. Add unit tests only for hard-to-reach edge cases.

## Messaging Workflow

For outgoing events:

- Define an integration event in `Features/<Area>/Events`.
- Add business state and `OutboxMessage.FromIntegrationEvent(...)` in the same command handler.
- Let the outbox processor publish through `IPublisher`.

For incoming events:

- Define or reference the integration event contract.
- Add an `IIntegrationEventHandler<TEvent>`.
- Let `IntegrationEventConsumerService`, `IConsumer`, and the inbox processor handle delivery, deduplication, and dispatch.

## Telemetry Workflow

- Use normal `ILogger<T>` for logs.
- Configure OpenTelemetry through the `Telemetry` settings section.
- Use `Console` exporter only for local checks.
- Use `Otlp` exporter for Seq, Grafana, Azure Monitor, or an OpenTelemetry Collector.
- Put custom activity names and tag keys in `Infrastructure/Telemetry`, not inline in handlers.

## Testing Guidance

Prefer:

1. Functional tests for HTTP behavior.
2. Integration tests for handlers, DI, database, outbox, and inbox behavior.
3. Unit tests only for isolated edge cases that are difficult to cover through integration tests.

Do not add broad unit-test coverage just to mirror implementation details.

## Questions To Ask Before Editing

Ask or infer these before implementing:

- Is this HTTP-facing, message-driven, or both?
- Is this a command, query, or integration event handler?
- Does it need persistence?
- Does it publish an integration event?
- Does it consume an integration event?
- Should failures be returned as `Result<T>` or handled as unexpected exceptions?
- Does this need a feature failure or problem title?
- Does this need a custom telemetry span or tag?
- Should behavior be tested through functional, integration, or unit tests?

## Agent Rules

- Follow the existing slice shape before inventing new folders.
- Keep endpoint code thin.
- Keep business workflow in handlers.
- Keep external-system code in `Infrastructure`.
- Use `Result<T>` for expected business outcomes.
- Use the existing command/query handlers instead of MediatR.
- Use centralized failures, log classes, and telemetry constants.
- Prefer integration and functional tests over unit tests.
