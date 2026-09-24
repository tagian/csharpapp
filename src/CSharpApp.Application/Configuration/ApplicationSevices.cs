
namespace CSharpApp.Application.Configuration;
public static class ApplicationServiceExtensions
{
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services)
    {
        services.AddScoped<IProductsService, ProductsService>();
        services.AddScoped<GetProductsQueryHandler>();
        services.AddScoped<GetProductByIdQueryHandler>();
        services.AddScoped<CreateProductCommandHandler>();

        services.AddScoped<ICategoriesService, CategoriesService>();
        services.AddScoped<GetCategoriesQueryHandler>();
        services.AddScoped<GetCategoryByIdQueryHandler>();
        services.AddScoped<CreateCategoryCommandHandler>();

        return services;
    }
}