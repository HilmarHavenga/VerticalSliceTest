namespace VerticalSliceTest.Orders.Api.Common.Pipelines;

public interface IRequestHandler<in TRequest, TResponse>
{
    Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken);
}
