using CSharpApp.Application.Products.Commands.CreateProduct;

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
builder.Services.AddScoped<GetProductsQueryHandler>();
builder.Services.AddScoped<GetProductByIdQueryHandler>();
builder.Services.AddScoped<CreateProductCommandHandler>();



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


versionedEndpointRouteBuilder.MapGet("api/v{version:apiVersion}/getproducts", async (GetProductsQueryHandler handler) =>
    {
        var query = new GetProductsQuery();
        var result = await handler.HandleAsync(query);
        return result;
    })
    .WithName("GetProducts")
    .HasApiVersion(1.0);

versionedEndpointRouteBuilder.MapGet("api/v{version:apiVersion}/product/{id:int}", async (int id, GetProductByIdQueryHandler handler) =>
    {
        var query = new GetProductByIdQuery(id);
        var result = await handler.HandleAsync(query);
        return result is null ? Results.NotFound() : Results.Ok(result);
    })
    .WithName("GetProduct")
    .HasApiVersion(1.0);

versionedEndpointRouteBuilder.MapPost("api/v{version:apiVersion}/product", async (CreateProductRequest request, CreateProductCommandHandler handler) =>
    {
        var command = new CreateProductCommand(
            request.Title,
            request.Price,
            request.Description,
            request.CategoryId,
            request.Images);

        var product = await handler.HandleAsync(command);

        return Results.Created($"/product/{product.Id}",product);
    })
    .WithName("PostProduct")
    .HasApiVersion(1.0);


app.Run();