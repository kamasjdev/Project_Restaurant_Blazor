using NSubstitute;
using Restaurant.Application.Abstractions;
using Restaurant.Application.DTO;
using Restaurant.Application.Exceptions;
using Restaurant.Application.Services;
using Restaurant.Core.Entities;
using Restaurant.Core.Repositories;
using Restaurant.Core.ValueObjects;
using Shouldly;

namespace Restaurant.UnitTests.Services
{
    public class ProductSaleServiceTests
    {
        [Fact]
        public async Task should_add_product_sale()
        {
            var product = AddDefaultProduct();
            var dto = new AddProductSaleDto { Email = "email@email.com", ProductId = product.Id };

            await _productSaleService.AddAsync(dto);

            await _productSaleRepository.Received(1).AddAsync(Arg.Any<ProductSale>());
        }

        [Fact]
        public async Task given_invalid_addition_id_should_throw_an_exception()
        {
            var product = AddDefaultProduct();
            var additionId = Guid.NewGuid();
            var expectedException = new AdditionNotFoundException(additionId);
            var dto = new AddProductSaleDto { Email = "email@email.com", ProductId = product.Id, AdditionId = additionId };

            var exception = await Record.ExceptionAsync(() => _productSaleService.AddAsync(dto));

            exception.ShouldNotBeNull();
            exception.ShouldBeOfType(expectedException.GetType());
            exception.Message.ShouldBe(expectedException.Message);
            ((AdditionNotFoundException)exception).AdditionId.ShouldBe(expectedException.AdditionId);
        }

        [Fact]
        public async Task given_invalid_order_id_should_throw_an_exception()
        {
            var product = AddDefaultProduct();
            var orderId = Guid.NewGuid();
            var expectedException = new OrderNotFoundException(orderId);
            var dto = new AddProductSaleDto { Email = "email@email.com", ProductId = product.Id, OrderId = orderId };

            var exception = await Record.ExceptionAsync(() => _productSaleService.AddAsync(dto));

            exception.ShouldNotBeNull();
            exception.ShouldBeOfType(expectedException.GetType());
            exception.Message.ShouldBe(expectedException.Message);
            ((OrderNotFoundException)exception).OrderId.ShouldBe(expectedException.OrderId);
        }

        [Fact]
        public async Task given_invalid_product_id_should_throw_an_exception()
        {
            var productId = Guid.NewGuid();
            var expectedException = new ProductNotFoundException(productId);
            var dto = new AddProductSaleDto { Email = "email@email.com", ProductId = productId };

            var exception = await Record.ExceptionAsync(() => _productSaleService.AddAsync(dto));

            exception.ShouldNotBeNull();
            exception.ShouldBeOfType(expectedException.GetType());
            exception.Message.ShouldBe(expectedException.Message);
            ((ProductNotFoundException)exception).ProductId.ShouldBe(expectedException.ProductId);
        }

        [Fact]
        public async Task should_delete_product_sale()
        {
            var productSale = AddDefaultProductSale();

            await _productSaleService.DeleteAsync(productSale.Id);

            await _productSaleRepository.Received(1).DeleteAsync(productSale);
        }

        [Fact]
        public async Task given_invalid_product_sale_id_when_delete_should_throw_an_exception()
        {
            var productSaleId = Guid.NewGuid();
            var expectedException = new ProductSaleNotFoundException(productSaleId);

            var exception = await Record.ExceptionAsync(() => _productSaleService.DeleteAsync(productSaleId));

            exception.ShouldNotBeNull();
            exception.ShouldBeOfType(expectedException.GetType());
            exception.Message.ShouldBe(expectedException.Message);
            ((ProductSaleNotFoundException)exception).ProductSaleId.ShouldBe(expectedException.ProductSaleId);
        }

        [Fact]
        public async Task should_update_product_sale()
        {
            var productSale = AddDefaultProductSale();
            var product = AddDefaultProduct();
            var dto = new AddProductSaleDto { Id = productSale.Id, Email = "email@test.com", ProductId = product.Id };

            await _productSaleService.UpdateAsync(dto);

            await _productSaleRepository.Received(1).UpdateAsync(productSale);
            productSale.Product.ShouldNotBeNull();
            productSale.ProductId.Value.ShouldBe(product.Id.Value);
            productSale.Email.Value.ShouldBe(dto.Email);
            productSale.Addition.ShouldBeNull();
            productSale.AdditionId.ShouldBeNull();
        }

        [Fact]
        public async Task should_update_product_sale_and_add_addition_to_product_sale()
        {
            var productSale = AddDefaultProductSale();
            var product = AddDefaultProduct();
            var addition = AddDefaultAddition();
            var dto = new AddProductSaleDto { Id = productSale.Id, Email = "email@test.com", ProductId = product.Id, AdditionId = addition.Id };

            await _productSaleService.UpdateAsync(dto);

            await _productSaleRepository.Received(1).UpdateAsync(productSale);
            productSale.Product.ShouldNotBeNull();
            productSale.ProductId.Value.ShouldBe(product.Id.Value);
            productSale.Email.Value.ShouldBe(dto.Email);
            productSale.Addition.ShouldNotBeNull();
            productSale.Addition.Id.ShouldBe(addition.Id);
            productSale.AdditionId.ShouldBe(addition.Id);
        }

        [Fact]
        public async Task should_update_product_sale_with_addition()
        {
            var productSale = AddDefaultProductSale(addition: AddDefaultAddition());
            var product = AddDefaultProduct();
            var addition = AddDefaultAddition();
            var dto = new AddProductSaleDto { Id = productSale.Id, Email = "email@test.com", ProductId = product.Id, AdditionId = addition.Id };

            await _productSaleService.UpdateAsync(dto);

            await _productSaleRepository.Received(1).UpdateAsync(productSale);
            productSale.Product.ShouldNotBeNull();
            productSale.ProductId.Value.ShouldBe(product.Id.Value);
            productSale.Email.Value.ShouldBe(dto.Email);
            productSale.Addition.ShouldNotBeNull();
            productSale.AdditionId.Value.ShouldBe(addition.Id.Value);
        }

        [Fact]
        public async Task should_update_product_sale_and_remove_addition()
        {
            var productSale = AddDefaultProductSale(addition: AddDefaultAddition());
            var product = AddDefaultProduct();
            var dto = new AddProductSaleDto { Id = productSale.Id, Email = "email@test.com", ProductId = product.Id };

            await _productSaleService.UpdateAsync(dto);

            await _productSaleRepository.Received(1).UpdateAsync(productSale);
            productSale.Product.ShouldNotBeNull();
            productSale.ProductId.Value.ShouldBe(product.Id.Value);
            productSale.Email.Value.ShouldBe(dto.Email);
            productSale.Addition.ShouldBeNull();
            productSale.AdditionId.ShouldBeNull();
        }

        [Fact]
        public async Task given_invalid_product_sale_id_when_update_should_throw_an_exception()
        {
            var productSaleId = Guid.NewGuid();
            var expectedException = new ProductSaleNotFoundException(productSaleId);
            var dto = new AddProductSaleDto { Id = productSaleId, Email = "email@test.com", ProductId = Guid.NewGuid() };

            var exception = await Record.ExceptionAsync(() => _productSaleService.UpdateAsync(dto));

            exception.ShouldNotBeNull();
            exception.ShouldBeOfType(expectedException.GetType());
            exception.Message.ShouldBe(expectedException.Message);
            ((ProductSaleNotFoundException)exception).ProductSaleId.ShouldBe(expectedException.ProductSaleId);
        }

        [Fact]
        public async Task given_ordered_product_sale_when_update_should_throw_an_exception()
        {
            var productSale = AddDefaultProductSale(ProductSaleState.Ordered);
            var expectedException = new CannotUpdateProductSaleException(productSale.Id, productSale.ProductSaleState.ToString());
            var dto = new AddProductSaleDto { Id = productSale.Id, Email = "email@test.com", ProductId = productSale.ProductId };

            var exception = await Record.ExceptionAsync(() => _productSaleService.UpdateAsync(dto));

            exception.ShouldNotBeNull();
            exception.ShouldBeOfType(expectedException.GetType());
            exception.Message.ShouldBe(expectedException.Message);
            ((CannotUpdateProductSaleException)exception).ProductSaleId.ShouldBe(expectedException.ProductSaleId);
            ((CannotUpdateProductSaleException)exception).ProductSaleState.ShouldBe(expectedException.ProductSaleState);
        }

        [Fact]
        public async Task given_invalid_product_id_when_update_should_throw_an_exception()
        {
            var productSale = AddDefaultProductSale();
            var productId = Guid.NewGuid();
            var expectedException = new ProductNotFoundException(productId);
            var dto = new AddProductSaleDto { Id = productSale.Id, Email = "email@test.com", ProductId = productId };

            var exception = await Record.ExceptionAsync(() => _productSaleService.UpdateAsync(dto));

            exception.ShouldNotBeNull();
            exception.ShouldBeOfType(expectedException.GetType());
            exception.Message.ShouldBe(expectedException.Message);
            ((ProductNotFoundException)exception).ProductId.ShouldBe(expectedException.ProductId);
        }

        [Fact]
        public async Task given_invalid_addition_id_when_update_should_throw_an_exception()
        {
            var productSale = AddDefaultProductSale();
            var additionId = Guid.NewGuid();
            var expectedException = new AdditionNotFoundException(additionId);
            var dto = new AddProductSaleDto { Id = productSale.Id, Email = "email@test.com", ProductId = productSale.ProductId, AdditionId = additionId };

            var exception = await Record.ExceptionAsync(() => _productSaleService.UpdateAsync(dto));

            exception.ShouldNotBeNull();
            exception.ShouldBeOfType(expectedException.GetType());
            exception.Message.ShouldBe(expectedException.Message);
            ((AdditionNotFoundException)exception).AdditionId.ShouldBe(expectedException.AdditionId);
        }

        private ProductSale AddDefaultProductSale(ProductSaleState productSaleState = ProductSaleState.New, Addition addition = null)
        {
            var product = AddDefaultProduct();
            var productSale = new ProductSale(Guid.NewGuid(), product, productSaleState, Email.Of("email@email.com"), addition);
            _productSaleRepository.GetAsync(productSale.Id).Returns(productSale);
            return productSale;
        }

        private Addition AddDefaultAddition()
        {
            var addition = new Addition(Guid.NewGuid(), "Addition#1", 10, AdditionKind.Drink);
            _additonRepository.GetAsync(addition.Id).Returns(addition);
            return addition;
        }

        private Product AddDefaultProduct()
        {
            var product = new Product(Guid.NewGuid(), "Product#1", 10, ProductKind.MainDish);
            _productRepository.GetAsync(product.Id).Returns(product);
            return product;
        }

        private readonly IProductSaleService _productSaleService;
        private readonly IProductSaleRepository _productSaleRepository;
        private readonly IProductRepository _productRepository;
        private readonly IAdditonRepository _additonRepository;
        private readonly IOrderRepository _orderRepository;

        public ProductSaleServiceTests()
        {
            _productSaleRepository = Substitute.For<IProductSaleRepository>();
            _productRepository = Substitute.For<IProductRepository>();
            _additonRepository = Substitute.For<IAdditonRepository>();
            _orderRepository = Substitute.For<IOrderRepository>();
            _productSaleService = new ProductSaleService(_productSaleRepository, _productRepository, _additonRepository, _orderRepository);
        }
    }

}
