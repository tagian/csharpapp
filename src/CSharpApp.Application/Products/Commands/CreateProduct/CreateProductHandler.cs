namespace CSharpApp.Application.Products.Commands.CreateProduct;

public sealed class CreateProductCommandHandler
{
    private readonly IProductsService _productsService;

    public CreateProductCommandHandler(IProductsService productsService)
    {
        _productsService = productsService;
    }

    public async Task<Product> HandleAsync(CreateProductCommand command)
    {
        var product = new CreateProductModel(
            command.Title,
            command.Price,
            command.Description,
            command.CategoryId,
            command.Images);

        return await _productsService.CreateProductAsync(product);
    }
}