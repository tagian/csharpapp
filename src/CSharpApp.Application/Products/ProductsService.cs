namespace CSharpApp.Application.Products;

public class ProductsService : IProductsService
{
    private readonly IHttpClientFactory _factory;
    private readonly RestApiSettings _restApiSettings;
    private readonly ILogger<ProductsService> _logger;

    public ProductsService(IOptions<RestApiSettings> restApiSettings, 
        ILogger<ProductsService> logger, IHttpClientFactory factory)
    {
        _factory = factory;
        _restApiSettings = restApiSettings.Value;
        _logger = logger;
    }

    public async Task<IReadOnlyCollection<Product>> GetProducts()
    {
        // _httpClient.BaseAddress = new Uri(_restApiSettings.BaseUrl!);
        var client = _factory.CreateClient("PlatziFakeStore");
        var response = await client.GetAsync(_restApiSettings.Products);
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        var res = JsonSerializer.Deserialize<List<Product>>(content);
        
        return res.AsReadOnly();
    }
}