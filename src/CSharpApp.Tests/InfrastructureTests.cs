using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using CSharpApp.Application.Products;
using CSharpApp.Core.Dtos;
using CSharpApp.Core.Exceptions;
using CSharpApp.Core.Interfaces;
using CSharpApp.Core.Settings;
using CSharpApp.Infrastructure.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace CSharpApp.Tests;

public class InfrastructureTests
{
    [Fact]
    public async Task TokenService_CachesAccessTokenUntilExpiry()
    {
        var token = CreateValidJwtToken(DateTime.UtcNow.AddMinutes(30));
        var handler = new RecordingHttpMessageHandler(new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent($"{{\"access_token\":\"{token}\",\"refresh_token\":\"refresh-456\"}}", Encoding.UTF8, "application/json")
        });

        var factory = new StubHttpClientFactory(handler, "https://example.test");
        var settings = Options.Create(new RestApiSettings
        {
            BaseUrl = "https://example.test",
            Auth = "/auth/login",
            Username = "user@example.com",
            Password = "secret"
        });

        var service = new global::TokenService(factory, settings);

        var first = await service.GetAccessTokenAsync();
        var second = await service.GetAccessTokenAsync();

        Assert.Equal(token, first);
        Assert.Equal(first, second);
        Assert.Equal(1, handler.CallCount);
    }

    [Fact]
    public async Task AuthHandler_AddsBearerTokenHeader()
    {
        var tokenService = new StubTokenService("abc-token");
        var innerHandler = new SuccessHttpMessageHandler();
        var handler = new AuthHandler(tokenService)
        {
            InnerHandler = innerHandler
        };

        var client = new HttpClient(handler);
        var request = new HttpRequestMessage(HttpMethod.Get, "https://example.test/products");

        var response = await client.SendAsync(request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(request.Headers.Authorization);
        Assert.Equal("Bearer", request.Headers.Authorization!.Scheme);
        Assert.Equal("abc-token", request.Headers.Authorization.Parameter);
    }

    [Fact]
    public async Task ProductsService_ThrowsUpstreamApiException_OnNonSuccessResponse()
    {
        var response = new HttpResponseMessage(HttpStatusCode.BadGateway)
        {
            Content = new StringContent("{\"error\":\"upstream failed\"}", Encoding.UTF8, "application/json")
        };

        var factory = new StubHttpClientFactory(new RecordingHttpMessageHandler(response), "https://example.test");
        var settings = Options.Create(new RestApiSettings
        {
            BaseUrl = "https://example.test",
            Products = "/products"
        });

        var logger = new FakeLogger<ProductsService>();
        var service = new ProductsService(settings, logger, factory);

        var exception = await Assert.ThrowsAsync<UpstreamApiException>(() => service.GetProducts());

        Assert.Equal(502, exception.StatusCode);
        Assert.Contains("upstream failed", exception.ResponseBody ?? string.Empty);
    }

    private sealed class StubHttpClientFactory : IHttpClientFactory
    {
        private readonly HttpMessageHandler _handler;
        private readonly string _baseUrl;

        public StubHttpClientFactory(HttpMessageHandler handler, string baseUrl)
        {
            _handler = handler;
            _baseUrl = baseUrl;
        }

        public HttpClient CreateClient(string name)
        {
            return new HttpClient(_handler, disposeHandler: false)
            {
                BaseAddress = new Uri(_baseUrl)
            };
        }
    }

    private sealed class RecordingHttpMessageHandler : HttpMessageHandler
    {
        private readonly HttpResponseMessage _response;

        public int CallCount { get; private set; }

        public RecordingHttpMessageHandler(HttpResponseMessage response)
        {
            _response = response;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            CallCount++;
            return Task.FromResult(_response);
        }
    }

    private sealed class SuccessHttpMessageHandler : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StringContent("OK");
            return Task.FromResult(response);
        }
    }

    private sealed class StubTokenService : ITokenService
    {
        private readonly string _token;

        public StubTokenService(string token)
        {
            _token = token;
        }

        public Task<string> GetAccessTokenAsync(CancellationToken ct = default)
        {
            return Task.FromResult(_token);
        }
    }

    private static string CreateValidJwtToken(DateTime expiresAt)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("super-secret-key-for-tests-1234567890"));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: "test-issuer",
            audience: "test-audience",
            claims: new[] { new Claim("sub", "user-1") },
            notBefore: DateTime.UtcNow.AddMinutes(-1),
            expires: expiresAt,
            signingCredentials: credentials);

        return tokenHandler.WriteToken(token);
    }

    private sealed class FakeLogger<T> : ILogger<T>
    {
        public IDisposable BeginScope<TState>(TState state) where TState : notnull => NullScope.Instance;
        public bool IsEnabled(LogLevel logLevel) => false;
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter) { }

        private sealed class NullScope : IDisposable
        {
            public static readonly NullScope Instance = new();
            public void Dispose() { }
        }
    }
}
