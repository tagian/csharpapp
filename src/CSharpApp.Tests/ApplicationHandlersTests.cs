using CSharpApp.Application.Categories.Commands.CreateCategory;
using CSharpApp.Application.Categories.Queries.GetCategories;
using CSharpApp.Application.Categories.Queries.GetCategoryById;
using CSharpApp.Application.Products.Commands.CreateProduct;
using CSharpApp.Application.Products.Queries.GetProductById;
using CSharpApp.Application.Products.Queries.GetProducts;
using CSharpApp.Core.Dtos;
using CSharpApp.Core.Interfaces;

namespace CSharpApp.Tests;

public class ApplicationHandlersTests
{
    [Fact]
    public async Task GetProductsQueryHandler_ReturnsProductsFromService()
    {
        var product = new Product { Id = 1, Title = "Widget", Price = 12, Description = "A sample product" };
        product.Images.Add("https://example.com/a.png");

        var stub = new StubProductsService([product]);
        var sut = new GetProductsQueryHandler(stub);

        var result = await sut.HandleAsync(new GetProductsQuery());

        var first = Assert.Single(result);
        Assert.Equal("Widget", first.Title);
    }

    [Fact]
    public async Task GetProductByIdQueryHandler_ReturnsMatchingProduct()
    {
        var product = new Product { Id = 7, Title = "Laptop", Price = 500, Description = "Gaming laptop" };
        product.Images.Add("https://example.com/laptop.png");

        var stub = new StubProductsService([product]);
        var sut = new GetProductByIdQueryHandler(stub);

        var result = await sut.HandleAsync(new GetProductByIdQuery(7));

        Assert.NotNull(result);
        Assert.Equal(7, result!.Id);
    }

    [Fact]
    public async Task CreateProductCommandHandler_MapsCommandToServiceModel()
    {
        var stub = new StubProductsService();
        var sut = new CreateProductCommandHandler(stub);

        var result = await sut.HandleAsync(new CreateProductCommand(
            "Mouse",
            25m,
            "Wireless mouse",
            2,
            ["a.png", "b.png"]));

        Assert.NotNull(result);
        Assert.Equal("Mouse", stub.LastCreatedProduct!.Title);
        Assert.Equal(25m, stub.LastCreatedProduct.Price);
        Assert.Equal(2, stub.LastCreatedProduct.CategoryId);
        Assert.Equal(2, stub.LastCreatedProduct.Images.Count);
    }

    [Fact]
    public async Task GetCategoriesQueryHandler_ReturnsCategoriesFromService()
    {
        var category = new Category { Id = 10, Name = "Electronics", Image = "https://example.com/electronics.png" };
        var stub = new StubCategoriesService([category]);
        var sut = new GetCategoriesQueryHandler(stub);

        var result = await sut.HandleAsync(new GetCategoriesQuery());

        var first = Assert.Single(result);
        Assert.Equal("Electronics", first.Name);
    }

    [Fact]
    public async Task CreateCategoryCommandHandler_MapsCommandToServiceModel()
    {
        var stub = new StubCategoriesService();
        var sut = new CreateCategoryCommandHandler(stub);

        var result = await sut.HandleAsync(new CreateCategoryCommand("Books", "https://example.com/books.png"));

        Assert.NotNull(result);
        Assert.Equal("Books", stub.LastCreatedCategory!.Name);
        Assert.Equal("https://example.com/books.png", stub.LastCreatedCategory.Image);
    }

    private sealed class StubProductsService : IProductsService
    {
        private readonly IReadOnlyCollection<Product> _products;

        public CreateProductModel? LastCreatedProduct { get; private set; }

        public StubProductsService(IEnumerable<Product>? products = null)
        {
            _products = products?.ToList() ?? [];
        }

        public Task<IReadOnlyCollection<Product>> GetProducts() => Task.FromResult(_products);

        public Task<Product?> GetProductByIdAsync(int id) =>
            Task.FromResult(_products.FirstOrDefault(p => p.Id == id));

        public Task<Product> CreateProductAsync(CreateProductModel product)
        {
            LastCreatedProduct = product;
            var created = new Product
            {
                Id = 99,
                Title = product.Title,
                Price = (int?)product.Price,
                Description = product.Description,
                Category = new Category { Id = product.CategoryId }
            };

            created.Images.AddRange(product.Images);
            return Task.FromResult(created);
        }
    }

    private sealed class StubCategoriesService : ICategoriesService
    {
        private readonly IReadOnlyCollection<Category> _categories;

        public CreateCategoryModel? LastCreatedCategory { get; private set; }

        public StubCategoriesService(IEnumerable<Category>? categories = null)
        {
            _categories = categories?.ToList() ?? [];
        }

        public Task<IReadOnlyCollection<Category>> GetCategories() => Task.FromResult(_categories);

        public Task<Category?> GetCategoryByIdAsync(int id) =>
            Task.FromResult(_categories.FirstOrDefault(c => c.Id == id));

        public Task<Category> CreateCategoryAsync(CreateCategoryModel product)
        {
            LastCreatedCategory = product;
            return Task.FromResult(new Category
            {
                Id = 100,
                Name = product.Name,
                Image = product.Image
            });
        }
    }
}
