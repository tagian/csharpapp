namespace CSharpApp.Infrastructure.Configuration;

public static class HttpConfiguration
{
    public static IServiceCollection AddHttpConfiguration(this IServiceCollection services)
    {
        services.AddHttpClient("PlatziFakeStore", client =>
        {
            client.BaseAddress = new Uri("https://api.escuelajs.co/api/v1/");
        });

        return services;
    }

}