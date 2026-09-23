namespace CSharpApp.Application.Products.Queries.GetProductById;

public sealed class GetProductByIdQueryHandler
{
    private readonly IProductsService _productService;
    public GetProductByIdQueryHandler(IProductsService productService)
    {
        _productService = productService;
    }

    public async Task<Product?> HandleAsync (GetProductByIdQuery query)
    {
        return await _productService.GetProductByIdAsync(query.id);
    }
}