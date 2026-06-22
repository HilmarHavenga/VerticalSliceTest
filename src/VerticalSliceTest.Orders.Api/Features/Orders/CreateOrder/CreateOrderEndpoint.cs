namespace VerticalSliceTest.Orders.Api.Features.Orders.CreateOrder;

public class CreateOrderEndpoint : IEndpoints
{
    public static void DefineEndpoints(IVersionedEndpointRouteBuilder app)
    {
        RouteGroupBuilder versioned = app.MapGroup("/api/v{version:apiVersion}/orders")
           //.RequireAuthorization()
           .AllowAnonymous()
           .HasApiVersion(Versions.V1)
           .ReportApiVersions();

        versioned.MapPost("/", CreateOrderAsync)
           .HasApiVersion(Versions.V1)
           .Produces(StatusCodes.Status201Created)
           .Produces(StatusCodes.Status400BadRequest)
           .Produces(StatusCodes.Status409Conflict)
           .WithName("CreateOrder");
    }

    internal static async Task<IResult> CreateOrderAsync(
        CreateOrderRequest orderRequest,
        ICommandHandler<CreateOrderRequest, Result<CreateOrderResponse>> handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.Handle(orderRequest, cancellationToken);

        return result.IsSuccess
            ? Results.Created($"/api/v1/orders/{result.Value.Id}", result.Value)
            : result.Error.ToFailureResult("Failed to create order");
    }
}
