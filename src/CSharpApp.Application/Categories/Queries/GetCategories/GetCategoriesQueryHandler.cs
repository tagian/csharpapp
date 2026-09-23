namespace CSharpApp.Application.Categories.Queries.GetCategories;

public sealed class GetCategoriesQueryHandler
{
    private readonly ICategoriesService _categoryService;
    public GetCategoriesQueryHandler(ICategoriesService categoryService)
    {
        _categoryService = categoryService;
    }

    public async Task<IReadOnlyCollection<Category>> HandleAsync (GetCategoriesQuery query)
    {
        return await _categoryService.GetCategories();
    }
}