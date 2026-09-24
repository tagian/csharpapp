namespace CSharpApp.Api.Endpoints;

public static class CategoryEndpoints
{
    public static void MapCategoryEndpoints(
        this IVersionedEndpointRouteBuilder versionedApi)
    {
        versionedApi.MapGet("api/v{version:apiVersion}/getcategories", async (GetCategoriesQueryHandler handler) =>
        {
            var query = new GetCategoriesQuery();
            var result = await handler.HandleAsync(query);
            return result;
        })
        .WithName("GetCategories")
        .HasApiVersion(1.0);

    versionedApi.MapGet("api/v{version:apiVersion}/category/{id:int}", async (int id, GetCategoryByIdQueryHandler handler) =>
        {
            var query = new GetCategoryByIdQuery(id);
            var result = await handler.HandleAsync(query);
            return result is null ? Results.NotFound() : Results.Ok(result);
        })
        .WithName("GetCategory")
        .HasApiVersion(1.0);

    versionedApi.MapPost("api/v{version:apiVersion}/category", async (CreateCategoryRequest request, CreateCategoryCommandHandler handler) =>
        {
            var command = new CreateCategoryCommand(
                request.Name,
                request.Image);

            var category = await handler.HandleAsync(command);

            return Results.Created($"/category/{category.Id}",category);
        })
        .WithName("PostCategory")
        .HasApiVersion(1.0);
    }
}