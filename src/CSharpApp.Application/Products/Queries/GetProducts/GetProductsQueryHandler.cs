namespace CSharpApp.Application.Products.Queries.GetProducts;

public sealed class GetProductsQueryHandler
{
    private readonly IProductsService _productService;
    public GetProductsQueryHandler(IProductsService productService)
    {
        _productService = productService;
    }

    public async Task<IReadOnlyCollection<Product>> HandleAsync (GetProductsQuery query)
    {
        return await _productService.GetProducts();
    }
}