


namespace CSharpApp.Application.Categories;

public class CategoriesService : ICategoriesService
{
    private readonly IHttpClientFactory _factory;
    private readonly RestApiSettings _restApiSettings;
    private readonly ILogger<CategoriesService> _logger;

    public CategoriesService(IOptions<RestApiSettings> restApiSettings, 
        ILogger<CategoriesService> logger, IHttpClientFactory factory)
    {
        _factory = factory;
        _restApiSettings = restApiSettings.Value;
        _logger = logger;
    }

    public async Task<IReadOnlyCollection<Category>> GetCategories()
    {
        var client = _factory.CreateClient("PlatziFakeStore");
        var response = await client.GetAsync(_restApiSettings.Categories);
        // response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            throw new UpstreamApiException(
            (int)response.StatusCode,
            body,
            response.Content.Headers.ContentType?.ToString());
        }
        
        var content = await response.Content.ReadAsStringAsync();
        var res = JsonSerializer.Deserialize<List<Category>>(content) ?? [];
        
        return res.AsReadOnly();
    }

    public async Task<Category?> GetCategoryByIdAsync(int id)
    {
        var client = _factory.CreateClient("PlatziFakeStore");
        var response = await client.GetAsync(
        $"{_restApiSettings.Categories}/{id}");
        // response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            throw new UpstreamApiException(
            (int)response.StatusCode,
            body,
            response.Content.Headers.ContentType?.ToString());
        }
        
        var content = await response.Content.ReadAsStringAsync();
        var res = JsonSerializer.Deserialize<Category>(content);
        
        return res;
    }

    public async Task<Category> CreateCategoryAsync(CreateCategoryModel Category)
    {
        var client = _factory.CreateClient("PlatziFakeStore");
        var response = await client.PostAsJsonAsync(_restApiSettings.Categories,Category);
        // response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            throw new UpstreamApiException(
            (int)response.StatusCode,
            body,
            response.Content.Headers.ContentType?.ToString());
        }
        var createdCategory = await response.Content.ReadFromJsonAsync<Category>();
        return createdCategory ?? throw new InvalidOperationException("The Categories API returned an empty response.");
    }
}