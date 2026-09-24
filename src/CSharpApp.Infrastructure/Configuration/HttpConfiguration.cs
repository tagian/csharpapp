public static class HttpConfiguration
{
    public static IServiceCollection AddHttpConfiguration(
        this IServiceCollection services)
    {
        //Use for endpoints without need for auth
        services
            .AddHttpClient("PlatziFakeStore", (serviceProvider, client) =>
            {
                var restApiSettings = serviceProvider
                    .GetRequiredService<IOptions<RestApiSettings>>()
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

        //GetAuth
        services.AddHttpClient("Auth", (serviceProvider, client) =>
        {
                var restApiSettings = serviceProvider
                    .GetRequiredService<IOptions<RestApiSettings>>()
                    .Value;

                client.BaseAddress = new Uri(restApiSettings.BaseUrl!);
        });

        //Use for endpoints with need for auth
        services
            .AddHttpClient("PlatziFakeStoreWithAuth", (serviceProvider, client) =>
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
            .AddHttpMessageHandler<AuthHandler>()
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