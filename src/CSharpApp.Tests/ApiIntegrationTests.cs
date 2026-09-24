using System.Net;
using System.Net.Http.Json;
using CSharpApp.Core.Dtos;
using CSharpApp.Core.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace CSharpApp.Tests;

public class ApiIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public ApiIntegrationTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetProductsEndpoint_ReturnsSerializedProducts()
    {
        var products = await _client.GetFromJsonAsync<List<Product>>("/api/v1/getproducts");

        Assert.NotNull(products);
        Assert.NotEmpty(products!);
        Assert.Contains(products!, p => p.Title == "Test product");
    }

    [Fact]
    public async Task GetProductByIdEndpoint_ReturnsSingleProduct()
    {
        var product = await _client.GetFromJsonAsync<Product>("/api/v1/product/99");

        Assert.NotNull(product);
        Assert.Equal(99, product!.Id);
        Assert.Equal("Test product", product.Title);
    }

    [Fact]
    public async Task GetProductByIdEndpoint_WhenMissing_ReturnsNotFound()
    {
        var response = await _client.GetAsync("/api/v1/product/404");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetCategoriesEndpoint_ReturnsSerializedCategories()
    {
        var categories = await _client.GetFromJsonAsync<List<Category>>("/api/v1/getcategories");

        Assert.NotNull(categories);
        Assert.NotEmpty(categories!);
        Assert.Contains(categories!, c => c.Name == "Test category");
    }

    [Fact]
    public async Task GetCategoryByIdEndpoint_ReturnsSingleCategory()
    {
        var category = await _client.GetFromJsonAsync<Category>("/api/v1/category/77");

        Assert.NotNull(category);
        Assert.Equal(77, category!.Id);
        Assert.Equal("Test category", category.Name);
    }

    [Fact]
    public async Task CreateProductEndpoint_AcceptsRequestAndReturnsCreatedProduct()
    {
        var payload = new
        {
            title = "Keyboard",
            price = 125,
            description = "Mechanical keyboard",
            categoryId = 2,
            images = new[] { "https://example.com/kb.png" }
        };

        var response = await _client.PostAsJsonAsync("/api/v1/product", payload);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var created = await response.Content.ReadFromJsonAsync<Product>();
        Assert.NotNull(created);
        Assert.Equal("Keyboard", created!.Title);
    }

    [Fact]
    public async Task CreateCategoryEndpoint_AcceptsRequestAndReturnsCreatedCategory()
    {
        var payload = new { name = "Books", image = "https://example.com/books.png" };

        var response = await _client.PostAsJsonAsync("/api/v1/category", payload);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var created = await response.Content.ReadFromJsonAsync<Category>();
        Assert.NotNull(created);
        Assert.Equal("Books", created!.Name);
    }
}

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");

        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll(typeof(IProductsService));
            services.RemoveAll(typeof(ICategoriesService));

            services.AddSingleton<IProductsService>(new StubProductsService(
            [
                new Product
                {
                    Id = 99,
                    Title = "Test product",
                    Price = 42,
                    Description = "Stubbed product from integration test"
                }
            ]));

            services.AddSingleton<ICategoriesService>(new StubCategoriesService(
            [
                new Category
                {
                    Id = 77,
                    Name = "Test category",
                    Image = "https://example.com/test-category.png"
                }
            ]));
        });
    }

    private sealed class StubProductsService : IProductsService
    {
        private readonly IReadOnlyCollection<Product> _products;

        public StubProductsService(IEnumerable<Product> products)
        {
            _products = products.ToList();
        }

        public Task<IReadOnlyCollection<Product>> GetProducts() => Task.FromResult(_products);

        public Task<Product?> GetProductByIdAsync(int id) =>
            Task.FromResult(_products.FirstOrDefault(p => p.Id == id));

        public Task<Product> CreateProductAsync(CreateProductModel product) =>
            Task.FromResult(new Product
            {
                Id = 1,
                Title = product.Title,
                Price = (int?)product.Price,
                Description = product.Description
            });
    }

    private sealed class StubCategoriesService : ICategoriesService
    {
        private readonly IReadOnlyCollection<Category> _categories;

        public StubCategoriesService(IEnumerable<Category> categories)
        {
            _categories = categories.ToList();
        }

        public Task<IReadOnlyCollection<Category>> GetCategories() => Task.FromResult(_categories);

        public Task<Category?> GetCategoryByIdAsync(int id) =>
            Task.FromResult(_categories.FirstOrDefault(c => c.Id == id));

        public Task<Category> CreateCategoryAsync(CreateCategoryModel category) =>
            Task.FromResult(new Category
            {
                Id = 1,
                Name = category.Name,
                Image = category.Image
            });
    }
}
