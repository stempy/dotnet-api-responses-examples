namespace ResponseExamples.Api.Services.ProductSamples;

/// <summary>
/// In-memory implementation of product service for demonstration purposes
/// </summary>
public class ProductService : IProductService
{
    private readonly List<Product> _products = new();
    private int _nextId = 1;

    public ProductService()
    {
        // Seed with some initial data
        _products.AddRange(new[]
        {
            new Product
            {
                Id = _nextId++,
                Name = "Laptop",
                Description = "High-performance laptop",
                Price = 999.99m,
                StockQuantity = 50,
                CreatedAt = DateTime.UtcNow.AddDays(-30)
            },
            new Product
            {
                Id = _nextId++,
                Name = "Mouse",
                Description = "Wireless ergonomic mouse",
                Price = 29.99m,
                StockQuantity = 200,
                CreatedAt = DateTime.UtcNow.AddDays(-15)
            },
            new Product
            {
                Id = _nextId++,
                Name = "Keyboard",
                Description = "Mechanical keyboard with RGB lighting",
                Price = 149.99m,
                StockQuantity = 75,
                CreatedAt = DateTime.UtcNow.AddDays(-20)
            }
        });
    }

    public Task<IEnumerable<Product>> GetAllAsync()
    {
        return Task.FromResult<IEnumerable<Product>>(_products);
    }

    public Task<Product?> GetByIdAsync(int id)
    {
        var product = _products.FirstOrDefault(p => p.Id == id);
        return Task.FromResult(product);
    }

    public Task<Product> CreateAsync(CreateProductRequest request)
    {
        var product = new Product
        {
            Id = _nextId++,
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            StockQuantity = request.StockQuantity,
            CreatedAt = DateTime.UtcNow
        };

        _products.Add(product);
        return Task.FromResult(product);
    }

    public Task<Product?> UpdateAsync(int id, UpdateProductRequest request)
    {
        var existingProduct = _products.FirstOrDefault(p => p.Id == id);
        if (existingProduct is null)
        {
            return Task.FromResult<Product?>(null);
        }

        var updatedProduct = existingProduct with
        {
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            StockQuantity = request.StockQuantity,
            UpdatedAt = DateTime.UtcNow
        };

        var index = _products.IndexOf(existingProduct);
        _products[index] = updatedProduct;

        return Task.FromResult<Product?>(updatedProduct);
    }

    public Task<bool> DeleteAsync(int id)
    {
        var product = _products.FirstOrDefault(p => p.Id == id);
        if (product is null)
        {
            return Task.FromResult(false);
        }

        _products.Remove(product);
        return Task.FromResult(true);
    }
}
