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
    public class OrderServiceTests
    {
        [Fact]
        public async Task should_create_order()
        {
            var dto = new AddOrderDto { Email = "email@email.com", Note = "this is note" };

            await _orderService.AddAsync(dto);

            await _orderRepository.Received(1).AddAsync(Arg.Any<Order>());
            await _unitOfWork.Received(1).BeginTransactionAsync();
            await _unitOfWork.Received(1).CommitAsync();
        }

        [Fact]
        public async Task given_invalid_product_sale_id_when_add_should_throw_an_exception()
        {
            var productSaleId = Guid.NewGuid();
            var dto = new AddOrderDto { Email = "email@email.test", ProductSaleIds = new List<Guid> { productSaleId } };
            var expectedException = new ProductSaleNotFoundException(productSaleId);

            var exception = await Record.ExceptionAsync(() => _orderService.AddAsync(dto));

            exception.ShouldNotBeNull();
            exception.ShouldBeOfType(expectedException.GetType());
            exception.Message.ShouldBe(expectedException.Message);
            ((ProductSaleNotFoundException)exception).ProductSaleId.ShouldBe(expectedException.ProductSaleId);
        }

        [Fact]
        public async Task should_delete_order()
        {
            var order = AddDefaultOrder();

            await _orderService.DeleteAsync(order.Id);

            await _orderRepository.Received(1).DeleteAsync(Arg.Any<Order>());
            await _unitOfWork.Received(1).BeginTransactionAsync();
            await _unitOfWork.Received(1).CommitAsync();
        }

        [Fact]
        public async Task given_invalid_order_id_when_delete_should_throw_an_exception()
        {
            var orderId = Guid.NewGuid();
            var expectedException = new OrderNotFoundException(orderId);

            var exception = await Record.ExceptionAsync(() => _orderService.DeleteAsync(orderId));

            exception.ShouldNotBeNull();
            exception.ShouldBeOfType(expectedException.GetType());
            exception.Message.ShouldBe(expectedException.Message);
            ((OrderNotFoundException)exception).OrderId.ShouldBe(expectedException.OrderId);
        }

        [Fact]
        public async Task should_update_order()
        {
            var order = AddDefaultOrder();
            var dto = new AddOrderDto { Id = order.Id, Email = "email@abc.com", Note = "atest" };

            await _orderService.UpdateAsync(dto);

            await _orderRepository.Received(1).UpdateAsync(order);
            order.Email.Value.ShouldBe(dto.Email);
            order.Note.ShouldBe(dto.Note);
            order.Products.ShouldBeEmpty();
            await _unitOfWork.Received(1).BeginTransactionAsync();
            await _unitOfWork.Received(1).CommitAsync();
        }

        [Fact]
        public async Task should_update_order_and_add_product_sale()
        {
            var order = AddDefaultOrder();
            var productSale = AddDefaultProductSale();
            var dto = new AddOrderDto { Id = order.Id, Email = "email@abc.com", Note = "atest", ProductSaleIds = new List<Guid> { productSale.Id } };

            await _orderService.UpdateAsync(dto);

            await _orderRepository.Received(1).UpdateAsync(order);
            order.Email.Value.ShouldBe(dto.Email);
            order.Note.ShouldBe(dto.Note);
            order.Products.ShouldNotBeEmpty();
            order.Products.Count().ShouldBe(1);
            order.Products.ShouldContain(p => p.Id == productSale.Id);
            order.Products.First().ProductSaleState.ShouldBe(ProductSaleState.Ordered);
            await _unitOfWork.Received(1).BeginTransactionAsync();
            await _unitOfWork.Received(1).CommitAsync();
        }

        [Fact]
        public async Task should_update_order_and_change_products()
        {
            var order = AddDefaultOrder(new List<ProductSale> { AddDefaultProductSale() });
            var previousProductSaleId = order.Products.First().Id;
            var productSale = AddDefaultProductSale();
            var dto = new AddOrderDto { Id = order.Id, Email = "email@abc.com", Note = "atest", ProductSaleIds = new List<Guid> { productSale.Id } };

            await _orderService.UpdateAsync(dto);

            await _orderRepository.Received(1).UpdateAsync(order);
            order.Email.Value.ShouldBe(dto.Email);
            order.Note.ShouldBe(dto.Note);
            order.Products.ShouldNotBeEmpty();
            order.Products.Count().ShouldBe(1);
            order.Products.ShouldNotContain(p => p.Id == previousProductSaleId);
            order.Products.ShouldContain(p => p.Id == productSale.Id);
            order.Products.First().ProductSaleState.ShouldBe(ProductSaleState.Ordered);
            await _unitOfWork.Received(1).BeginTransactionAsync();
            await _unitOfWork.Received(1).CommitAsync();
        }

        [Fact]
        public async Task given_invalid_product_sale_id_when_update_should_throw_an_exception()
        {
            var orderId = Guid.NewGuid();
            var expectedException = new OrderNotFoundException(orderId);
            var dto = new AddOrderDto { Id = orderId, Email = "email@abc.com", Note = "atest" };

            var exception = await Record.ExceptionAsync(() => _orderService.UpdateAsync(dto));

            exception.ShouldNotBeNull();
            exception.ShouldBeOfType(expectedException.GetType());
            exception.Message.ShouldBe(expectedException.Message);
            ((OrderNotFoundException)exception).OrderId.ShouldBe(expectedException.OrderId);
        }

        [Fact]
        public async Task given_invalid_order_id_when_update_should_throw_an_exception()
        {
            var order = AddDefaultOrder();
            var productSaleId = Guid.NewGuid();
            var expectedException = new ProductSaleNotFoundException(productSaleId);
            var dto = new AddOrderDto { Id = order.Id, Email = "email@abc.com", Note = "atest", ProductSaleIds = new List<Guid> { productSaleId } };

            var exception = await Record.ExceptionAsync(() => _orderService.UpdateAsync(dto));

            exception.ShouldNotBeNull();
            exception.ShouldBeOfType(expectedException.GetType());
            exception.Message.ShouldBe(expectedException.Message);
            ((ProductSaleNotFoundException)exception).ProductSaleId.ShouldBe(expectedException.ProductSaleId);
        }

        private Order AddDefaultOrder(IEnumerable<ProductSale> productSales = null)
        {
            var order = new Order(Guid.NewGuid(), "ORDER", _currentDate,
                productSales is not null ? productSales.Sum(p => p.EndPrice) : 0, Email.Of("email@email.com"), products: productSales);
            _orderRepository.GetAsync(order.Id).Returns(order);
            return order;
        }

        private ProductSale AddDefaultProductSale()
        {
            var product = AddDefaultProduct();
            var productSale = new ProductSale(Guid.NewGuid(), product, ProductSaleState.Ordered, Email.Of("email@email.com"));
            _productSaleRepository.GetAsync(productSale.Id).Returns(productSale);
            return productSale;
        }

        private Product AddDefaultProduct()
        {
            var product = new Product(Guid.NewGuid(), "Product#1", 10, ProductKind.MainDish);
            return product;
        }

        private readonly DateTime _currentDate;
        private readonly IOrderService _orderService;
        private readonly IOrderRepository _orderRepository;
        private readonly IProductSaleRepository _productSaleRepository;
        private readonly IClock _clock;
        private readonly IUnitOfWork _unitOfWork;

        public OrderServiceTests()
        {
            _currentDate = new DateTime(2022, 9, 13, 17, 55, 10);
            _orderRepository = Substitute.For<IOrderRepository>();
            _productSaleRepository = Substitute.For<IProductSaleRepository>();
            _clock = Substitute.For<IClock>();
            _clock.CurrentDate().Returns(_currentDate);
            _unitOfWork = Substitute.For<IUnitOfWork>();
            _orderService = new OrderService(_orderRepository, _productSaleRepository, _clock, _unitOfWork);
        }
    }
}
