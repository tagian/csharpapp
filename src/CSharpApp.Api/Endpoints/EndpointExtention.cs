namespace CSharpApp.Api.Endpoints;

public static class EndpointExtensions
{
    public static void MapApiEndpoints(this WebApplication app)
    {
        var versionedApi = app.NewVersionedApi();

        versionedApi.MapProductEndpoints();
        versionedApi.MapCategoryEndpoints();
    }
}