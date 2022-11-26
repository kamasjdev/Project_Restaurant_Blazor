using NSubstitute;
using Restaurant.Application.Abstractions;
using Restaurant.Application.DTO;
using Restaurant.Application.Exceptions;
using Restaurant.Application.Services;
using Restaurant.Core.Entities;
using Restaurant.Core.Exceptions;
using Restaurant.Core.Repositories;
using Restaurant.Core.ValueObjects;
using Shouldly;

namespace Restaurant.UnitTests.Services
{
    public class ProductServiceTests
    {
        [Fact]
        public async Task should_create_product()
        {
            var product = new ProductDto { ProductName = "Product #1", Price = 20, ProductKind = ProductKind.Soup.ToString() };

            await _productService.AddAsync(product);

            await _productRepository.Received(1).AddAsync(Arg.Any<Product>());
        }

        [Fact]
        public async Task should_update_product()
        {
            var product = AddDefaultProduct();
            var productDto = new ProductDto { Id = product.Id, ProductName = "Product #1", Price = 20, ProductKind = ProductKind.Soup.ToString() };

            await _productService.UpdateAsync(productDto);

            await _productRepository.Received(1).UpdateAsync(product);
        }

        [Fact]
        public async Task given_invalid_product_kind_should_throw_an_exception()
        {
            var product = AddDefaultProduct();
            var productDto = new ProductDto { Id = product.Id, ProductName = "Product #1", Price = 20, ProductKind = "test" };
            var expectedException = new InvalidProductKindException(productDto.ProductKind);

            var exception = await Record.ExceptionAsync(() => _productService.UpdateAsync(productDto));

            exception.ShouldNotBeNull();
            exception.ShouldBeOfType(expectedException.GetType());
            exception.Message.ShouldBe(expectedException.Message);
            ((InvalidProductKindException)exception).ProductKind.ShouldBe(productDto.ProductKind);
        }

        [Fact]
        public async Task given_invalid_id_when_update_should_throw_an_exception()
        {
            var productDto = new ProductDto { ProductName = "Product #1", Price = 20, ProductKind = ProductKind.Pizza.ToString() };
            var expectedException = new Application.Exceptions.ProductNotFoundException(productDto.Id);

            var exception = await Record.ExceptionAsync(() => _productService.UpdateAsync(productDto));

            exception.ShouldNotBeNull();
            exception.ShouldBeOfType(expectedException.GetType());
            exception.Message.ShouldBe(expectedException.Message);
            ((Application.Exceptions.ProductNotFoundException)exception).ProductId.ShouldBe(expectedException.ProductId);
        }

        [Fact]
        public async Task should_delete_product()
        {
            var addition = AddDefaultProduct();

            await _productService.DeleteAsync(addition.Id);

            await _productRepository.Received(1).DeleteAsync(addition);
        }

        [Fact]
        public async Task given_invalid_id_when_delete_should_throw_an_exception()
        {
            var id = Guid.NewGuid();
            var expectedException = new Application.Exceptions.ProductNotFoundException(id);

            var exception = await Record.ExceptionAsync(() => _productService.DeleteAsync(id));

            exception.ShouldNotBeNull();
            exception.ShouldBeOfType(expectedException.GetType());
            exception.Message.ShouldBe(expectedException.Message);
            ((Application.Exceptions.ProductNotFoundException)exception).ProductId.ShouldBe(expectedException.ProductId);
        }

        [Fact]
        public async Task given_product_ordered_when_delete_should_throw_an_exception()
        {
            var product = AddDefaultProduct(new List<EntityId> { new EntityId(Guid.NewGuid()) });
            var expectedException = new CannotDeleteProductOrderedException(product.Id);

            var exception = await Record.ExceptionAsync(() => _productService.DeleteAsync(product.Id));

            exception.ShouldNotBeNull();
            exception.ShouldBeOfType(expectedException.GetType());
            exception.Message.ShouldBe(expectedException.Message);
            ((CannotDeleteProductOrderedException)exception).ProductId.ShouldBe(expectedException.ProductId);
        }

        private Product AddDefaultProduct(IEnumerable<EntityId> productSaleIds = null)
        {
            var product = new Product(Guid.NewGuid(), "Product#1", 10, ProductKind.MainDish, productSaleIds: productSaleIds);
            _productRepository.GetAsync(product.Id).Returns(product);
            return product;
        }

        private readonly IProductService _productService;
        private readonly IProductRepository _productRepository;

        public ProductServiceTests()
        {
            _productRepository = Substitute.For<IProductRepository>();
            _productService = new ProductService(_productRepository);
        }
    }
}
