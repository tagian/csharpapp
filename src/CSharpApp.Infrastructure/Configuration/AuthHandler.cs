using System.Net.Http.Headers;

public sealed class AuthHandler : DelegatingHandler
{
    private readonly ITokenService _tokenService;

    public AuthHandler(ITokenService tokenService)
    {
        _tokenService = tokenService;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,CancellationToken cancellationToken)
    {
        var token = await _tokenService.GetAccessTokenAsync(cancellationToken);

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        return await base.SendAsync(request, cancellationToken);
    }
}