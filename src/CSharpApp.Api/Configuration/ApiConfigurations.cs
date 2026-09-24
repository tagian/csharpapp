namespace CSharpApp.Api.Configuration;
public static class ApiConfigurationExtensions
{
    public static IServiceCollection AddApiConfiguration(
        this IServiceCollection services,
        IConfiguration configuration)
    {

        services.Configure<RestApiSettings>(configuration!.GetSection(nameof(RestApiSettings)));
        services.Configure<HttpClientSettings>(configuration.GetSection(nameof(HttpClientSettings)));

        services.AddOpenApi("v1");

        services.AddHttpConfiguration();

        services.AddExceptionHandler<UpstreamExceptionHandler>();
        services.AddProblemDetails();

        services
            .AddApiVersioning()
            .AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });

        return services;
    }
}