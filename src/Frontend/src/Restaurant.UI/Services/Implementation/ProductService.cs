using Restaurant.Shared.ProductProto;
using Restaurant.UI.DTO;
using Restaurant.UI.Services.Abstractions;

namespace Restaurant.UI.Services.Implementation
{
    internal sealed class ProductService : IProductService
    {
        private readonly Products.ProductsClient _productsClient;

        public ProductService(Products.ProductsClient productsClient)
        {
            _productsClient = productsClient;
        }

        public async Task<Guid> AddAsync(ProductDto productDto)
        {
            var response = await _productsClient.AddProductAsync(new Product
            {
                Id = productDto.Id.ToString(),
                Price = productDto.Price.ToString(),
                ProductKind = productDto.ProductKind,
                ProductName = productDto.ProductName
            });
            return Guid.Parse(response.Id);
        }

        public async Task DeleteAsync(Guid id)
        {
            await _productsClient.DeleteProductAsync(new DeleteProductRequest { Id = id.ToString() });
        }

        public async Task<IEnumerable<ProductDto>> GetAllAsync()
        {
            return (await _productsClient.GetProductsAsync(new Google.Protobuf.WellKnownTypes.Empty()))
                .Products.Select(p => new ProductDto
                {
                    Id = Guid.Parse(p.Id),
                    ProductName = p.ProductName,
                    Price = decimal.Parse(p.Price),
                    ProductKind = p.ProductKind
                });
        }

        public async Task<ProductDto?> GetAsync(Guid id)
        {
            var product = await _productsClient.GetProductAsync(new GetProductRequest
            {
                Id = id.ToString()
            });
            return product is not null ? new ProductDto
            {
                Id = Guid.Parse(product.Id),
                ProductKind = product.ProductKind,
                ProductName = product.ProductName,
                Price = decimal.Parse(product.Price)
            } : null;
        }

        public async Task UpdateAsync(ProductDto productDto)
        {
            await _productsClient.UpdateProductAsync(new Product
            {
                Id = productDto.Id.ToString(),
                Price = productDto.Price.ToString(),
                ProductKind = productDto.ProductKind,
                ProductName = productDto.ProductName,
            });
        }
    }
}
