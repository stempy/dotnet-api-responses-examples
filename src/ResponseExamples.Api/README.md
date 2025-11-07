# Product CRUD REST API

This project demonstrates a complete RESTful CRUD API implementation using ASP.NET Core Minimal APIs with proper HTTP status codes, ProblemDetails error handling, and comprehensive OpenAPI documentation.

## Features

? **Full CRUD Operations** - Create, Read, Update, Delete products  
? **RESTful Design** - Proper HTTP verbs and status codes  
? **Minimal APIs** - Modern, streamlined endpoint definitions  
? **Separated Concerns** - Endpoints in separate file from Program.cs  
? **ProblemDetails** - Standardized error responses (RFC 7807 & RFC 9457)  
? **Validation Errors** - MVC-style ValidationProblemDetails structure for 400 errors  
? **Error Examples** - Comprehensive error handling demonstrations for all common HTTP error codes  
? **OpenAPI 3.0** - Full API documentation using Microsoft.AspNetCore.OpenApi  
? **Typed Results** - Strong typing for HTTP responses  
? **Dependency Injection** - Service-based architecture  
? **Validation** - Input validation with appropriate error responses  

## API Endpoint Groups

### Product API (`/api/products`)
Full CRUD operations for managing products. See detailed documentation below.

### Error Examples API (`/api/errors`)
Demonstrates standardized error responses for various HTTP status codes (4xx and 5xx).
See **[ERROR_EXAMPLES.md](ERROR_EXAMPLES.md)** for comprehensive documentation including:
- 400 Bad Request (with ValidationProblemDetails)
- 401 Unauthorized
- 403 Forbidden
- 404 Not Found
- 409 Conflict
- 422 Unprocessable Entity
- 429 Too Many Requests
- 500 Internal Server Error
- 503 Service Unavailable

## API Endpoints

### Get All Products
```http
GET /api/products
```
**Response:** `200 OK`
```json
[
  {
    "id": 1,
    "name": "Laptop",
    "description": "High-performance laptop",
    "price": 999.99,
    "stockQuantity": 50,
    "createdAt": "2024-01-01T10:00:00Z",
    "updatedAt": null
  }
]
```

### Get Product by ID
```http
GET /api/products/{id}
```
**Response:** `200 OK` or `404 Not Found`
```json
{
  "id": 1,
  "name": "Laptop",
  "description": "High-performance laptop",
  "price": 999.99,
  "stockQuantity": 50,
  "createdAt": "2024-01-01T10:00:00Z",
  "updatedAt": null
}
```

**Error Response (404):**
```json
{
  "title": "Product not found",
  "detail": "Product with ID 999 was not found",
  "status": 404,
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.5"
}
```

### Create Product
```http
POST /api/products
Content-Type: application/json

{
  "name": "New Product",
  "description": "Product description",
  "price": 99.99,
  "stockQuantity": 100
}
```
**Response:** `201 Created`
```json
{
  "id": 4,
  "name": "New Product",
  "description": "Product description",
  "price": 99.99,
  "stockQuantity": 100,
  "createdAt": "2024-01-15T14:30:00Z",
  "updatedAt": null
}
```
**Location Header:** `/api/products/4`

**Validation Error Response (400):**
```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "Name": ["The Name field is required."],
    "Price": ["The Price field must be greater than or equal to 0."]
  }
}
```

### Update Product
```http
PUT /api/products/{id}
Content-Type: application/json

{
  "name": "Updated Product",
  "description": "Updated description",
  "price": 149.99,
  "stockQuantity": 75
}
```
**Response:** `200 OK` or `404 Not Found` or `400 Bad Request`
```json
{
  "id": 1,
  "name": "Updated Product",
  "description": "Updated description",
  "price": 149.99,
  "stockQuantity": 75,
  "createdAt": "2024-01-01T10:00:00Z",
  "updatedAt": "2024-01-15T15:00:00Z"
}
```

**Validation Error Response (400):**
```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "StockQuantity": ["The StockQuantity field must be greater than or equal to 0."]
  }
}
```

### Delete Product
```http
DELETE /api/products/{id}
```
**Response:** `204 No Content` or `404 Not Found`

## HTTP Status Codes

| Status Code | Usage |
|------------|-------|
| `200 OK` | Successful GET or PUT request |
| `201 Created` | Successful POST request (resource created) |
| `204 No Content` | Successful DELETE request |
| `400 Bad Request` | Invalid input data or validation failure |
| `404 Not Found` | Resource not found |

## Error Response Formats

### 404 Not Found (ProblemDetails)
```json
{
  "title": "Product not found",
  "detail": "Product with ID 123 was not found",
  "status": 404,
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.5"
}
```

### 400 Validation Errors (ValidationErrors)
Follows the same structure as MVC's ValidationProblemDetails:
```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "Name": ["The Name field is required."],
    "Price": ["The Price field must be greater than or equal to 0."],
    "StockQuantity": ["The StockQuantity field must be greater than or equal to 0."]
  }
}
```

## Project Structure

```
ResponseExamples.Api/
??? Program.cs                          # Application entry point
??? README.md                           # Project overview and documentation
??? ERROR_EXAMPLES.md                   # Comprehensive error handling examples
??? GenerateOpenApiDocument.ps1         # Script to generate openapi.json
??? Services/
?   ??? ProductEndpoints.cs            # Product CRUD endpoints with OpenAPI metadata
?   ??? ErrorDemoEndpoints.cs           # Error demonstration endpoints
?   ??? Product.cs                     # Product models and DTOs
?   ??? IProductService.cs             # Product service interface
?   ??? ProductService.cs              # In-memory product service
```

## Running the Application

```bash
dotnet run
```

The API will be available at:
- HTTPS: `https://localhost:5001`
- HTTP: `http://localhost:5000`

## OpenAPI Documentation

The API uses the new Microsoft OpenAPI libraries (not Swashbuckle) for generating OpenAPI 3.0 documentation.

### Accessing the OpenAPI Document

In development mode, the OpenAPI document is available at:
- **OpenAPI JSON**: `https://localhost:5001/openapi/v1.json`

### Generating OpenAPI Document at Build Time

To generate an `openapi.json` file, you can use the provided PowerShell script:

```powershell
# Run from the project directory
pwsh GenerateOpenApiDocument.ps1
```

This will:
1. Build the application
2. Start the application temporarily
3. Fetch the OpenAPI document from the running app
4. Save it to `openapi.json`
5. Stop the application

### OpenAPI Features

All endpoints include:
- **Summary** - Short description of the endpoint
- **Description** - Detailed explanation of what the endpoint does
- **Response Types** - Typed response schemas for all status codes
- **Parameter Descriptions** - Documentation for route parameters
- **Request/Response Examples** - Automatically generated from typed results

## Key Implementation Details

### Microsoft OpenAPI
The API uses Microsoft's official OpenAPI libraries:
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
```

### OpenAPI Extensions on Endpoints
Each endpoint uses `WithOpenApi()` to provide rich documentation:
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

## Testing Examples

### Viewing OpenAPI Document

```bash
# Get OpenAPI JSON document
curl https://localhost:5001/openapi/v1.json

# Save to file
curl https://localhost:5001/openapi/v1.json -o openapi.json
```

### Using curl

```bash
# Get all products
curl -X GET https://localhost:5001/api/products

# Get specific product
curl -X GET https://localhost:5001/api/products/1

# Create product
curl -X POST https://localhost:5001/api/products \
  -H "Content-Type: application/json" \
  -d '{"name":"Headphones","description":"Wireless headphones","price":79.99,"stockQuantity":150}'

# Update product
curl -X PUT https://localhost:5001/api/products/1 \
  -H "Content-Type: application/json" \
  -d '{"name":"Updated Laptop","description":"Updated description","price":1099.99,"stockQuantity":45}'

# Delete product
curl -X DELETE https://localhost:5001/api/products/1
```

### Using PowerShell

```powershell
# Get all products
Invoke-RestMethod -Uri "https://localhost:5001/api/products" -Method Get

# Create product
$body = @{
    name = "Monitor"
    description = "4K Monitor"
    price = 399.99
    stockQuantity = 25
} | ConvertTo-Json

Invoke-RestMethod -Uri "https://localhost:5001/api/products" -Method Post -Body $body -ContentType "application/json"
```

## Next Steps

Consider adding:
- [ ] Entity Framework Core for database persistence
- [ ] Authentication and Authorization
- [ ] Rate limiting
- [ ] Pagination for GET all products
- [ ] Filtering and sorting
- [ ] API versioning
- [ ] PATCH endpoint for partial updates
- [ ] Caching with ETag support
- [ ] Unit and integration tests

## Technologies Used

- **.NET 10** - Latest .NET framework
- **ASP.NET Core Minimal APIs** - Streamlined endpoint definitions
- **Microsoft.AspNetCore.OpenApi** - Official Microsoft OpenAPI 3.0 support
- **ProblemDetails (RFC 7807)** - Standardized error responses
- **Dependency Injection** - Built-in DI container
