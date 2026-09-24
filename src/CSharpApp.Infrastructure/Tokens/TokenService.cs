
public sealed class TokenService : ITokenService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly RestApiSettings _settings;

    private string? _accessToken;
    private string? _refreshToken;
    private DateTimeOffset _expiresAt;

    private readonly SemaphoreSlim _lock = new(1, 1);

    public  TokenService(
        IHttpClientFactory httpClientFactory,
        IOptions<RestApiSettings> options)
    {
        _httpClientFactory = httpClientFactory;
        _settings = options.Value;
    }

    public async Task<string> GetAccessTokenAsync(
        CancellationToken cancellationToken = default)
    {
        // Still valid? Just return it.
        if (!string.IsNullOrWhiteSpace(_accessToken) &&
            DateTimeOffset.UtcNow < _expiresAt)
        {
            return _accessToken;
        }

        await _lock.WaitAsync(cancellationToken);

        try
        {
            // Check again because another request may have refreshed it.
            if (!string.IsNullOrWhiteSpace(_accessToken) &&
                DateTimeOffset.UtcNow < _expiresAt)
            {
                return _accessToken;
            }

            // if (!string.IsNullOrWhiteSpace(_refreshToken)) I dont have a refresh endpoint
            // {
            //     var refreshed = await TryRefreshAsync(cancellationToken);

            //     if (refreshed)
            //         return _accessToken!;
            // }

            await AuthenticateAsync(cancellationToken);

            return _accessToken!;
        }
        finally
        {
            _lock.Release();
        }
    }

    private async Task AuthenticateAsync(CancellationToken cancellationToken)
    {
        var client = _httpClientFactory.CreateClient("Auth");

        var request = new
        {
            email = _settings.Username,
            password = _settings.Password
        };

        var response = await client.PostAsJsonAsync(
            _settings.Auth,
            request,
            cancellationToken);

        response.EnsureSuccessStatusCode();

        var result =
            await response.Content.ReadFromJsonAsync<AuthResponse>(cancellationToken: cancellationToken) ?? throw new InvalidOperationException( "Authentication response was empty.");

        SaveTokens(result);
    }

    // I dont have a refresh endpoint
    // private async Task<bool> TryRefreshAsync(CancellationToken cancellationToken)
    // { 
    //     var client = _httpClientFactory.CreateClient(" Auth");

    //     var request = new
    //     {
    //         refreshToken = _refreshToken
    //     };

    //     var response = await client.PostAsJsonAsync(
    //         "auth/refresh",
    //         request,
    //         cancellationToken);

    //     if (!response.IsSuccessStatusCode)
    //         return false;

    //     var result =
    //         await response.Content.ReadFromJsonAsync<AuthResponse>(
    //             cancellationToken: cancellationToken);

    //     if (result is null)
    //         return false;

    //     SaveTokens(result);

    //     return true;
    // }

    private void SaveTokens(AuthResponse result)
    {
        _accessToken = result.AccessToken;
        _refreshToken = result.RefreshToken;
    
        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(result.AccessToken);
    
        _expiresAt = jwt.ValidTo.AddMinutes(-1);
    }
}