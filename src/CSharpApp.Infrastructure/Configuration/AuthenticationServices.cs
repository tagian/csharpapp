namespace CSharpApp.Infrastructure.Configuration;
public static class AuthenticationServiceExtensions
{
    public static IServiceCollection AddAuthenticationServices(this IServiceCollection services)
    {
        services.AddSingleton<ITokenService, TokenService>();
        services.AddTransient<AuthHandler>();

        return services;
    }
}