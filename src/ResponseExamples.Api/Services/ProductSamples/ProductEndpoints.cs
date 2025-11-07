using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace ResponseExamples.Api.Services.ProductSamples;

public static class ProductEndpoints
{
    public static IEndpointRouteBuilder MapProductEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/products")
            .WithTags("Products")
            .WithOpenApi();

        // GET: /api/products - Get all products
        group.MapGet("/", GetAllProducts)
            .WithName("GetAllProducts")
            .WithSummary("Get all products")
            .WithDescription("Retrieves a list of all products in the inventory")
            .Produces<IEnumerable<Product>>(StatusCodes.Status200OK)
            .WithOpenApi(operation =>
            {
                operation.Summary = "Get all products";
                operation.Description = "Retrieves a list of all products in the inventory";
                return operation;
            });

        // GET: /api/products/{id} - Get product by ID
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

        // POST: /api/products - Create a new product
        group.MapPost("/", CreateProduct)
            .WithName("CreateProduct")
            .WithSummary("Create a new product")
            .WithDescription("Creates a new product in the inventory and returns the created product")
            .Produces<Product>(StatusCodes.Status201Created)
            .Produces<ValidationProblemDetails>(StatusCodes.Status400BadRequest)
            .WithOpenApi(operation =>
            {
                operation.Summary = "Create a new product";
                operation.Description = "Creates a new product in the inventory and returns the created product";
                return operation;
            });

        // PUT: /api/products/{id} - Update an existing product
        group.MapPut("/{id:int}", UpdateProduct)
            .WithName("UpdateProduct")
            .WithSummary("Update an existing product")
            .WithDescription("Updates an existing product with new information")
            .Produces<Product>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
            .Produces<ValidationProblemDetails>(StatusCodes.Status400BadRequest)
            .WithOpenApi(operation =>
            {
                operation.Summary = "Update an existing product";
                operation.Description = "Updates an existing product with new information";
                operation.Parameters[0].Description = "The unique identifier of the product to update";
                return operation;
            });

        // DELETE: /api/products/{id} - Delete a product
        group.MapDelete("/{id:int}", DeleteProduct)
            .WithName("DeleteProduct")
            .WithSummary("Delete a product")
            .WithDescription("Deletes a product from the inventory")
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
            .WithOpenApi(operation =>
            {
                operation.Summary = "Delete a product";
                operation.Description = "Deletes a product from the inventory";
                operation.Parameters[0].Description = "The unique identifier of the product to delete";
                return operation;
            });

        return app;
    }

    /// <summary>
    /// Gets all products from the inventory
    /// </summary>
    private static async Task<Ok<IEnumerable<Product>>> GetAllProducts(
        IProductService productService)
    {
        var products = await productService.GetAllAsync();
        return TypedResults.Ok(products);
    }

    /// <summary>
    /// Gets a specific product by ID
    /// </summary>
    private static async Task<Results<Ok<Product>, NotFound<ProblemDetails>>> GetProductById(
        int id,
        IProductService productService)
    {
        var product = await productService.GetByIdAsync(id);
        
        if (product is null)
        {
            return TypedResults.NotFound(new ProblemDetails
            {
                Title = "Product not found",
                Detail = $"Product with ID {id} was not found",
                Status = StatusCodes.Status404NotFound,
                Type = "https://tools.ietf.org/html/rfc9110#section-15.5.5"
            });
        }

        return TypedResults.Ok(product);
    }

    /// <summary>
    /// Creates a new product
    /// </summary>
    private static async Task<Results<CreatedAtRoute<Product>, BadRequest<ValidationProblemDetails>>> CreateProduct(
        CreateProductRequest request,
        IProductService productService,
        ILogger<Program> logger)
    {
        // Validate request and collect all errors
        var errors = new Dictionary<string, string[]>();

        if (string.IsNullOrWhiteSpace(request.Name))
        {
            errors["Name"] = new[] { "The Name field is required." };
        }

        if (request.Price < 0)
        {
            errors["Price"] = new[] { "The Price field must be greater than or equal to 0." };
        }

        if (request.StockQuantity < 0)
        {
            errors["StockQuantity"] = new[] { "The StockQuantity field must be greater than or equal to 0." };
        }

        if (errors.Count > 0)
        {
            return TypedResults.BadRequest(new ValidationProblemDetails(errors));
        }

        var product = await productService.CreateAsync(request);
        
        logger.LogInformation("Created product with ID {ProductId}", product.Id);

        return TypedResults.CreatedAtRoute(
            product,
            routeName: nameof(GetProductById),
            routeValues: new { id = product.Id });
    }

    /// <summary>
    /// Updates an existing product
    /// </summary>
    private static async Task<Results<Ok<Product>, NotFound<ProblemDetails>, BadRequest<ValidationProblemDetails>>> UpdateProduct(
        int id,
        UpdateProductRequest request,
        IProductService productService,
        ILogger<Program> logger)
    {
        // Validate request and collect all errors
        var errors = new Dictionary<string, string[]>();

        if (string.IsNullOrWhiteSpace(request.Name))
        {
            errors["Name"] = new[] { "The Name field is required." };
        }

        if (request.Price < 0)
        {
            errors["Price"] = new[] { "The Price field must be greater than or equal to 0." };
        }

        if (request.StockQuantity < 0)
        {
            errors["StockQuantity"] = new[] { "The StockQuantity field must be greater than or equal to 0." };
        }

        if (errors.Count > 0)
        {
            return TypedResults.BadRequest(new ValidationProblemDetails(errors));
        }

        var product = await productService.UpdateAsync(id, request);
        
        if (product is null)
        {
            return TypedResults.NotFound(new ProblemDetails
            {
                Title = "Product not found",
                Detail = $"Product with ID {id} was not found",
                Status = StatusCodes.Status404NotFound,
                Type = "https://tools.ietf.org/html/rfc9110#section-15.5.5"
            });
        }

        logger.LogInformation("Updated product with ID {ProductId}", id);

        return TypedResults.Ok(product);
    }

    /// <summary>
    /// Deletes a product
    /// </summary>
    private static async Task<Results<NoContent, NotFound<ProblemDetails>>> DeleteProduct(
        int id,
        IProductService productService,
        ILogger<Program> logger)
    {
        var deleted = await productService.DeleteAsync(id);
        
        if (!deleted)
        {
            return TypedResults.NotFound(new ProblemDetails
            {
                Title = "Product not found",
                Detail = $"Product with ID {id} was not found",
                Status = StatusCodes.Status404NotFound,
                Type = "https://tools.ietf.org/html/rfc9110#section-15.5.5"
            });
        }

        logger.LogInformation("Deleted product with ID {ProductId}", id);

        return TypedResults.NoContent();
    }
}
