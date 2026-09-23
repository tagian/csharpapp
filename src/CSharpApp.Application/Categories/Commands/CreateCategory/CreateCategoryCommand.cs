namespace CSharpApp.Application.Categories.Commands.CreateCategory;

public sealed record CreateCategoryCommand(
    string Name,
    string Image);