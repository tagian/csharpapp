namespace CSharpApp.Application.Categories.Commands.CreateCategory;

public sealed class CreateCategoryCommandHandler
{
    private readonly ICategoriesService _categoriesService;

    public CreateCategoryCommandHandler(ICategoriesService categoriesService)
    {
        _categoriesService = categoriesService;
    }

    public async Task<Category> HandleAsync(CreateCategoryCommand command)
    {
        var category = new CreateCategoryModel(
            command.Name,
            command.Image);

        return await _categoriesService.CreateCategoryAsync(category);
    }
}