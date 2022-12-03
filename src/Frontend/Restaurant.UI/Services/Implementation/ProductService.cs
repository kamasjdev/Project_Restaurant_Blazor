using Restaurant.UI.DTO;
using Restaurant.UI.Services.Abstractions;

namespace Restaurant.UI.Services.Implementation
{
    internal sealed class ProductService : IProductService
    {
        private readonly List<ProductDto> _products = new()
        {
            new ProductDto { Id = Guid.NewGuid(), ProductName = "Product#1", Price= 100M, ProductKind = "MainDish" },
            new ProductDto { Id = Guid.NewGuid(), ProductName = "Product#2", Price= 50M, ProductKind = "Soup" },
            new ProductDto { Id = Guid.NewGuid(), ProductName = "Product#3", Price= 20M, ProductKind = "Pizza" },
            new ProductDto { Id = Guid.NewGuid(), ProductName = "Product#4", Price= 150M, ProductKind = "MainDish" },
            new ProductDto { Id = Guid.NewGuid(), ProductName = "Product#5", Price= 25M, ProductKind = "Pizza" },
        };

        public Task<Guid> AddAsync(ProductDto productDto)
        {
            productDto.Id = Guid.NewGuid();
            _products.Add(productDto);
            return Task.FromResult(productDto.Id);
        }

        public Task DeleteAsync(Guid id)
        {
            var product = _products.SingleOrDefault(p => p.Id == id);

            if (product is null)
            {
                return Task.CompletedTask;
            }

            _products.Remove(product);
            return Task.CompletedTask;
        }

        public Task<IEnumerable<ProductDto>> GetAllAsync()
        {
            return Task.FromResult<IEnumerable<ProductDto>>(_products);
        }

        public Task<ProductDto?> GetAsync(Guid id)
        {
            return Task.FromResult(_products.SingleOrDefault(p => p.Id == id));
        }

        public Task UpdateAsync(ProductDto productDto)
        {
            return Task.CompletedTask;
        }
    }
}
