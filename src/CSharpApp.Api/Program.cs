using CSharpApp.Application.Categories;
using CSharpApp.Core.Interfaces;


var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(builder.Configuration).CreateLogger();

builder.Logging.ClearProviders();
builder.Services.AddSerilog(Log.Logger);

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

builder.Services.AddScoped<ICategoriesService, CategoriesService>();
builder.Services.AddScoped<GetCategoriesQueryHandler>();
builder.Services.AddScoped<GetCategoryByIdQueryHandler>();
builder.Services.AddScoped<CreateCategoryCommandHandler>();



var app = builder.Build();

app.UseSerilogRequestLogging(options =>
{
    options.MessageTemplate =
        "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0} ms";
});

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

    versionedEndpointRouteBuilder.MapGet("api/v{version:apiVersion}/getcategories", async (GetCategoriesQueryHandler handler) =>
    {
        var query = new GetCategoriesQuery();
        var result = await handler.HandleAsync(query);
        return result;
    })
    .WithName("GetCategories")
    .HasApiVersion(1.0);

versionedEndpointRouteBuilder.MapGet("api/v{version:apiVersion}/category/{id:int}", async (int id, GetCategoryByIdQueryHandler handler) =>
    {
        var query = new GetCategoryByIdQuery(id);
        var result = await handler.HandleAsync(query);
        return result is null ? Results.NotFound() : Results.Ok(result);
    })
    .WithName("GetCategory")
    .HasApiVersion(1.0);

versionedEndpointRouteBuilder.MapPost("api/v{version:apiVersion}/category", async (CreateCategoryRequest request, CreateCategoryCommandHandler handler) =>
    {
        var command = new CreateCategoryCommand(
            request.Name,
            request.Image);

        var category = await handler.HandleAsync(command);

        return Results.Created($"/category/{category.Id}",category);
    })
    .WithName("PostCategory")
    .HasApiVersion(1.0);



app.Run();