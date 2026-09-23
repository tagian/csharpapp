namespace CSharpApp.Application.Products.Commands.CreateProduct;

public sealed record CreateProductCommand(
    string Title,
    decimal Price,
    string Description,
    int CategoryId,
    IReadOnlyCollection<string> Images);