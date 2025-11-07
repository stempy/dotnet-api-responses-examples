# Migration from Swashbuckle to Microsoft OpenAPI

## Summary

This project has been successfully migrated from Swashbuckle.AspNetCore to the official Microsoft.AspNetCore.OpenApi library.

## Changes Made

### 1. Package References (ResponseExamples.Api.csproj)
- **Removed**: `Swashbuckle.AspNetCore` v9.0.6
- **Added**: `Microsoft.AspNetCore.OpenApi` v9.0.0

### 2. Program.cs Updates
**Before (Swashbuckle):**
```csharp
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Product API",
        Version = "v1",
        Description = "A RESTful API for managing products with full CRUD operations"
    });
});

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Product API v1");
    c.RoutePrefix = string.Empty;
});
```

**After (Microsoft OpenAPI):**
```csharp
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

app.MapOpenApi(); // Maps to /openapi/v1.json by default
```

### 3. ProductEndpoints.cs Enhancements
All endpoints now include `WithOpenApi()` extensions for enhanced documentation:

```csharp
group.MapGet("/{id:int}", GetProductById)
    .WithName("GetProductById")
    .WithSummary("Get a product by ID")
    .WithDescription("Retrieves a specific product by its unique identifier")
    .Produces<Product>(StatusCodes.Status200OK)
    .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
    .WithOpenApi(operation =>
    {
        operation.Summary = "Get a product by ID";
        operation.Description = "Retrieves a specific product by its unique identifier";
        operation.Parameters[0].Description = "The unique identifier of the product";
        return operation;
    });
```

### 4. New Files Created

#### GenerateOpenApiDocument.ps1
PowerShell script to generate `openapi.json` file:
- Builds the application
- Starts it temporarily
- Fetches the OpenAPI document from /openapi/v1.json
- Saves it to a file
- Stops the application

Usage:
```powershell
pwsh GenerateOpenApiDocument.ps1
```

### 5. Build Integration
Added MSBuild target to provide instructions for OpenAPI generation:
```xml
<Target Name="GenerateOpenApiDocument" AfterTargets="Build" Condition="'$(Configuration)' == 'Debug'">
  <Message Text="OpenAPI document can be accessed at runtime via /openapi/v1.json" Importance="high" />
  <Message Text="To generate openapi.json file, run: pwsh GenerateOpenApiDocument.ps1" Importance="high" />
</Target>
```

## Key Differences: Swashbuckle vs Microsoft OpenAPI

| Feature | Swashbuckle | Microsoft OpenAPI |
|---------|-------------|-------------------|
| UI | Built-in Swagger UI | No built-in UI (document only) |
| Endpoint | `/swagger/v1/swagger.json` | `/openapi/v1.json` |
| Configuration | `AddSwaggerGen()` | `AddOpenApi()` |
| Activation | `UseSwagger()`, `UseSwaggerUI()` | `MapOpenApi()` |
| OpenAPI Version | 3.0 | 3.0 |
| Endpoint Metadata | Attributes or fluent API | `WithOpenApi()` fluent API |
| Document Generation | Runtime only | Runtime (build-time via script) |

## Benefits of Microsoft OpenAPI

1. **Official Microsoft Support**: Part of the ASP.NET Core framework
2. **Better Integration**: Designed specifically for Minimal APIs
3. **Type Safety**: Better support for typed results
4. **Modern API**: Uses the latest .NET patterns
5. **Lighter Weight**: No UI overhead if you don't need it
6. **Document Transformers**: Flexible document customization
7. **First-class Minimal API Support**: Built with Minimal APIs in mind

## OpenAPI Document Access

### Development Mode
The OpenAPI document is automatically available at:
- `http://localhost:5000/openapi/v1.json`
- `https://localhost:5001/openapi/v1.json`

### Generating Static File
Run the provided PowerShell script:
```powershell
pwsh GenerateOpenApiDocument.ps1
```

This creates `openapi.json` in the project directory.

## Viewing the OpenAPI Document

### Option 1: Swagger Editor (Online)
1. Go to https://editor.swagger.io/
2. File ? Import URL
3. Enter: `http://localhost:5000/openapi/v1.json` (with app running)
   OR
4. File ? Import File ? Select generated `openapi.json`

### Option 2: Swagger UI (Docker)
```bash
docker run -p 8080:8080 -e SWAGGER_JSON_URL=http://host.docker.internal:5000/openapi/v1.json swaggerapi/swagger-ui
```

### Option 3: VS Code Extension
Install "OpenAPI (Swagger) Editor" extension and open `openapi.json`

### Option 4: Redoc
```bash
npx @redocly/cli preview-docs openapi.json
```

## Validation

The generated OpenAPI document is valid OpenAPI 3.0.1 specification and includes:

? All endpoints (GET, POST, PUT, DELETE)
? Request/response schemas
? Status codes and response types
? Parameter descriptions
? Component schemas (Product, CreateProductRequest, UpdateProductRequest, ProblemDetails, ValidationProblemDetails)
? Proper tagging
? Summary and description metadata

## Testing the Migration

1. **Build the project:**
   ```bash
   dotnet build
   ```

2. **Run the application:**
   ```bash
   dotnet run
   ```

3. **Access the OpenAPI document:**
   ```bash
   curl http://localhost:5000/openapi/v1.json
   ```

4. **Generate static file:**
   ```powershell
   pwsh GenerateOpenApiDocument.ps1
   ```

## Next Steps (Optional)

If you need a UI for your API documentation, consider:

1. **Scalar**: Modern, beautiful API documentation UI
   ```bash
   dotnet add package Scalar.AspNetCore
   ```

2. **Swagger UI (separate)**: Install UI separately
   ```bash
   dotnet add package Swashbuckle.AspNetCore.SwaggerUI
   ```

3. **ReDoc**: Alternative documentation UI
   ```bash
   npm install -g @redocly/cli
   redocly preview-docs openapi.json
   ```

4. **Custom UI**: Build your own using the OpenAPI document

## Compatibility

- ? .NET 10
- ? .NET 9
- ? .NET 8 (requires Microsoft.AspNetCore.OpenApi)
- ? OpenAPI 3.0.1 specification
- ? All standard OpenAPI tools and viewers
