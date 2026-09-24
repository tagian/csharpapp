
namespace CSharpApp.Api.Endpoints;

public static class ProductEndpoints
{
    public static void MapProductEndpoints(
        this IVersionedEndpointRouteBuilder versionedApi)
    {

        versionedApi.MapGet("api/v{version:apiVersion}/getproducts", async (GetProductsQueryHandler handler) =>
            {
                var query = new GetProductsQuery();
                var result = await handler.HandleAsync(query);
                return result;
            })
            .WithName("GetProducts")
            .HasApiVersion(1.0);

        versionedApi.MapGet("api/v{version:apiVersion}/product/{id:int}", async (int id, GetProductByIdQueryHandler handler) =>
            {
                var query = new GetProductByIdQuery(id);
                var result = await handler.HandleAsync(query);
                return result is null ? Results.NotFound() : Results.Ok(result);
            })
            .WithName("GetProduct")
            .HasApiVersion(1.0);

        versionedApi.MapPost("api/v{version:apiVersion}/product", async (CreateProductRequest request, CreateProductCommandHandler handler) =>
            {
                var command = new CreateProductCommand(
                    request.Title,
                    request.Price,
                    request.Description,
                    request.CategoryId,
                    request.Images);

                var product = await handler.HandleAsync(command);

                return Results.Created($"/product/{product.Id}",product);
            })
            .WithName("PostProduct")
            .HasApiVersion(1.0);
    }
}