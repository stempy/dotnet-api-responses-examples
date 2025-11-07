using Microsoft.OpenApi;
using ResponseExamples.Api.Services.ErrorEndpoints;
using ResponseExamples.Api.Services.ProductSamples;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddEndpointsApiExplorer();

// Add OpenAPI services
builder.Services.AddOpenApi(options =>
{
    // set runtime build version to OpenAPI 3.0 for compatibility
    options.OpenApiVersion = OpenApiSpecVersion.OpenApi3_0; // default with .NET10 is OpenAPI 3.1, set to 3.0 for compatibility
    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        document.Info = new()
        {
            Title = "Product API",
            Version = "v1",
            Description = "A RESTful API for managing products with full CRUD operations"
        };
        return Task.CompletedTask;
    });
});

// Add ProblemDetails service for standardized error responses
builder.Services.AddProblemDetails();

// Register application services
builder.Services.AddSingleton<IProductService, ProductService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Use ProblemDetails middleware for exception handling
app.UseExceptionHandler();
app.UseStatusCodePages();

app.UseHttpsRedirection();

// Map Product CRUD endpoints
app.MapProductEndpoints();

// Map Error Demo endpoints
app.MapErrorDemoEndpoints();

app.Run();
