using ResponseExamples.Api.Services.ErrorEndpoints;
using ResponseExamples.Api.Services.ProductSamples;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddEndpointsApiExplorer();

// Add OpenAPI services
builder.Services.AddOpenApi(options =>
{
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
