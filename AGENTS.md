# AGENTS.md

- Read `REPO_MAP.md` before editing.
- Focus on feature slices under `src/VerticalSliceTest.Orders.Api/Features/<Area>/<UseCase>`.
- Keep endpoints thin; put workflow in command/query handlers.
- Use `ICommandHandler<TCommand, Result<TResponse>>` for writes and `IQueryHandler<TQuery, Result<TResponse>>` for reads.
- Use validators for request validation; use `Result<T>` and centralized failures for expected business failures.
- Put shared usings in `src/VerticalSliceTest.Orders.Api/Usings.cs`.
- Keep EF Core, RabbitMQ, caching, telemetry, clock, outbox, and inbox details in `Infrastructure`.
- Use `OutboxMessage.FromIntegrationEvent(...)` for outgoing integration events.
- Use centralized log classes and telemetry constants; avoid scattered strings.
- Prefer integration and functional tests. Use unit tests only for hard-to-reach edge cases.
- Do not introduce MediatR.
