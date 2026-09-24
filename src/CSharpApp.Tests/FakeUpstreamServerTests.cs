using System.Net;
using System.Text;
using CSharpApp.Application.Products;
using CSharpApp.Core.Dtos;
using CSharpApp.Core.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CSharpApp.Tests;

public class FakeUpstreamServerTests
{
    [Fact]
    public async Task ProductsService_CanSendRequestToFakeUpstreamServer()
    {
        using var server = new HttpListener();
        server.Prefixes.Add("http://localhost:5123/");
        server.Start();

        var requestTask = Task.Run(async () =>
        {
            var context = await server.GetContextAsync();
            var response = context.Response;
            var payload = "[{\"id\":1,\"title\":\"Desk\",\"price\":250,\"description\":\"Work desk\",\"images\":[\"https://example.com/desk.png\"]}]";
            var buffer = Encoding.UTF8.GetBytes(payload);
            response.StatusCode = 200;
            response.ContentType = "application/json";
            response.ContentLength64 = buffer.Length;
            await response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
            response.OutputStream.Close();
        });

        var settings = Options.Create(new RestApiSettings
        {
            BaseUrl = "http://localhost:5123",
            Products = "/products"
        });

        var service = new ProductsService(settings, new FakeLogger<ProductsService>(), new HttpClientFactoryWithBaseAddress("http://localhost:5123"));
        var result = await service.GetProducts();

        await requestTask;

        var first = Assert.Single(result);
        Assert.Equal("Desk", first.Title);
        Assert.Equal(250, first.Price);

        server.Stop();
    }

    private sealed class HttpClientFactoryWithBaseAddress : IHttpClientFactory
    {
        private readonly string _baseUrl;

        public HttpClientFactoryWithBaseAddress(string baseUrl)
        {
            _baseUrl = baseUrl;
        }

        public HttpClient CreateClient(string name)
        {
            return new HttpClient
            {
                BaseAddress = new Uri(_baseUrl)
            };
        }
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
