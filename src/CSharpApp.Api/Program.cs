
var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(builder.Configuration).CreateLogger();

builder.Logging.ClearProviders();
builder.Services.AddSerilog(Log.Logger);

builder.Services.AddApiConfiguration(builder.Configuration);
builder.Services.AddApplicationServices();
builder.Services.AddAuthenticationServices();

var app = builder.Build();

app.UseSerilogRequestLogging(options =>
{
    options.MessageTemplate =
        "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0} ms";
});

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "1");
    });
}

// app.UseHttpsRedirection();

app.MapApiEndpoints();

app.Run();

public partial class Program { }