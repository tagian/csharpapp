
var builder = WebApplication.CreateBuilder(args);

var logger = new LoggerConfiguration().ReadFrom.Configuration(builder.Configuration).CreateLogger();
builder.Logging.ClearProviders().AddSerilog(logger);

builder.Services.AddOpenApi("v1");
builder.Services.AddDefaultConfiguration(builder.Configuration);
builder.Services.AddHttpConfiguration();
builder.Services.AddProblemDetails();
builder.Services.AddApiVersioning().AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "1");
    });
}

// app.UseHttpsRedirection();

var versionedEndpointRouteBuilder = app.NewVersionedApi();

versionedEndpointRouteBuilder.MapGet("api/v{version:apiVersion}/getproducts", async (IProductsService productsService) =>
    {
        var products = await productsService.GetProducts();
        return products;
    })
    .WithName("GetProducts")
    .HasApiVersion(1.0);

app.Run();