namespace CSharpApp.Application.Categories.Queries.GetCategoryById;

public sealed class GetCategoryByIdQueryHandler
{
    private readonly ICategoriesService _productService;
    public GetCategoryByIdQueryHandler(ICategoriesService productService)
    {
        _productService = productService;
    }

    public async Task<Category?> HandleAsync (GetCategoryByIdQuery query)
    {
        return await _productService.GetCategoryByIdAsync(query.id);
    }
}