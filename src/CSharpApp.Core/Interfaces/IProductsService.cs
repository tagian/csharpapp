namespace CSharpApp.Core.Interfaces;

public interface IProductsService
{
    Task<IReadOnlyCollection<Product>> GetProducts();
    Task<Product?> GetProductByIdAsync(int id);
    Task<Product> CreateProductAsync(CreateProductModel product);
}