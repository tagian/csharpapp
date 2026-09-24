// namespace CSharpApp.Infrastructure.Configuration;

// public static class HttpConfiguration
// {
//     public static IServiceCollection AddHttpConfiguration(
//         this IServiceCollection services,)
//     {
//         services.AddHttpClient("PlatziFakeStore", client =>
//         {
//             client.BaseAddress = new Uri("https://api.escuelajs.co/api/v1/");
//                             new Uri(configuration["UpstreamApi:BaseUrl"]!);

//         });

//         return services;
//     }

// }


public static class HttpConfiguration
{
    public static IServiceCollection AddHttpConfiguration(
        this IServiceCollection services)
    {
        services
            .AddHttpClient("PlatziFakeStore", (serviceProvider, client) =>
            {
                var restApiSettings = serviceProvider
                    .GetRequiredService<IOptions<RestApiSettings>>()
                    .Value;

                var httpClientSettings = serviceProvider
                    .GetRequiredService<IOptions<HttpClientSettings>>()
                    .Value;

                client.BaseAddress = new Uri(restApiSettings.BaseUrl!);
            })
            .ConfigurePrimaryHttpMessageHandler(serviceProvider =>
            {
                var settings = serviceProvider
                    .GetRequiredService<IOptions<HttpClientSettings>>()
                    .Value;

                return new SocketsHttpHandler
                {
                    PooledConnectionLifetime = TimeSpan.FromMinutes(settings.LifeTime)
                };
            })
            .AddResilienceHandler("retry", (pipeline, context) =>
            {
                var settings = context.ServiceProvider
                    .GetRequiredService<IOptions<HttpClientSettings>>()
                    .Value;

                pipeline.AddRetry(new HttpRetryStrategyOptions
                {
                    MaxRetryAttempts = settings.RetryCount,
                    Delay = TimeSpan.FromMilliseconds(settings.SleepDuration),
                    BackoffType = DelayBackoffType.Constant,
                    UseJitter = false
                });
            });

        return services;
    }
}