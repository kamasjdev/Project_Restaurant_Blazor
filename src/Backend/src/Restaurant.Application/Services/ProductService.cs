using Restarant.Application.Mappings;
using Restaurant.Application.Abstractions;
using Restaurant.Application.DTO;
using Restaurant.Application.Exceptions;
using Restaurant.Core.Entities;
using Restaurant.Core.Repositories;

namespace Restaurant.Application.Services
{
    internal sealed class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task AddAsync(ProductDto productDto)
        {
            productDto.Id = Guid.NewGuid();
            var product = new Product(productDto.Id, productDto.ProductName, productDto.Price, productDto.ProductKind);
            await _productRepository.AddAsync(product);
        }

        public async Task DeleteAsync(Guid id)
        {
            var product = await _productRepository.GetAsync(id);

            if (product is null)
            {
                throw new ProductNotFoundException(id);
            }

            if (product.ProductSaleIds.Any())
            {
                throw new CannotDeleteProductOrderedException(id);
            }

            await _productRepository.DeleteAsync(product);
        }

        public async Task<IEnumerable<ProductDto>> GetAllAsync()
        {
            return (await _productRepository.GetAllAsync()).Select(p => p.AsDto());
        }

        public async Task<ProductDetailsDto?> GetAsync(Guid id)
        {
            return (await _productRepository.GetAsync(id))?.AsDetailsDto();
        }

        public async Task UpdateAsync(ProductDto productDto)
        {
            var product = await _productRepository.GetAsync(productDto.Id);

            if (product is null)
            {
                throw new ProductNotFoundException(productDto.Id);
            }

            product.ChangeProductName(productDto.ProductName);
            product.ChangePrice(productDto.Price);
            product.ChangeProductKind(productDto.ProductKind);

            await _productRepository.UpdateAsync(product);
        }
    }

}
