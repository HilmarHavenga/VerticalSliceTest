namespace VerticalSliceTest.Orders.Api.Common.Endpoints;

internal static class EndpointExtensions
{
    public static void UseEndpoints<TMarker>(this IEndpointRouteBuilder app)
    {
        IEnumerable<TypeInfo> endpointTypes = GetEndpointTypesFromAssembly<TMarker>(typeof(TMarker));

        foreach (TypeInfo endpointType in endpointTypes)
        {
            endpointType.GetMethod(nameof(IEndpoints.DefineEndpoints))?
                .Invoke(null, [app.NewVersionedApi(endpointType.Name)]);
        }
    }

    internal static IEnumerable<TypeInfo> GetEndpointTypesFromAssembly<TMarker>(Type typeMarker)
    {
        return typeMarker.Assembly.DefinedTypes
            .Where(type => !type.IsAbstract && !type.IsInterface && typeof(IEndpoints).IsAssignableFrom(type));
    }
}
