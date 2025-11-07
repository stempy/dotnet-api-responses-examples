namespace ResponseExamples.Api.Services.ProductSamples;

/// <summary>
/// Represents a product in the inventory
/// </summary>
public record Product
{
    public int Id { get; init; }
    public required string Name { get; init; }
    public string? Description { get; init; }
    public decimal Price { get; init; }
    public int StockQuantity { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}

/// <summary>
/// DTO for creating a new product
/// </summary>
public record CreateProductRequest
{
    public required string Name { get; init; }
    public string? Description { get; init; }
    public decimal Price { get; init; }
    public int StockQuantity { get; init; }
}

/// <summary>
/// DTO for updating an existing product
/// </summary>
public record UpdateProductRequest
{
    public required string Name { get; init; }
    public string? Description { get; init; }
    public decimal Price { get; init; }
    public int StockQuantity { get; init; }
}
