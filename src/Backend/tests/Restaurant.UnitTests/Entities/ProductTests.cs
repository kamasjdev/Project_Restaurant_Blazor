using Restaurant.Core.Entities;
using Restaurant.Core.Exceptions;
using Restaurant.Core.ValueObjects;
using Shouldly;

namespace Restaurant.UnitTests.Entities
{
    public class ProductTests
    {
        [Fact]
        public void should_create_product()
        {
            var price = 100M;
            var productName = "Product #1";

            var product = new Product(Guid.NewGuid(), productName, price, ProductKind.Soup);

            product.ShouldNotBeNull();
            product.Price.Value.ShouldBe(price);
            product.ProductName.Value.ShouldBe(productName);
        }

        [Theory]
        [InlineData("avsd")]
        [InlineData("10")]
        public void given_invalid_product_kind_should_throw_an_exception(string productKind)
        {
            var price = 100M;
            var productName = "Product #1";
            var expectedException = new InvalidProductKindException(productKind);

            var exception = Record.Exception(() => new Product(Guid.NewGuid(), productName, price, productKind));

            exception.ShouldNotBeNull();
            exception.ShouldBeOfType(expectedException.GetType());
            exception.Message.ShouldBe(expectedException.Message);
            ((InvalidProductKindException)exception).ProductKind.ShouldBe(expectedException.ProductKind);
        }

        [Fact]
        public void given_valid_orders_should_add()
        {
            var product = new Product(Guid.NewGuid(), "Product #1", 100M, ProductKind.MainDish);
            var orders = new List<Order>() { new Order(Guid.NewGuid(), "ORDER", DateTime.UtcNow, 100M, Email.Of("email@email.com")) };

            product.AddOrders(orders);

            product.Orders.ShouldNotBeEmpty();
            product.Orders.Count().ShouldBeGreaterThan(0);
        }

        [Fact]
        public void given_null_orders_should_throw_an_exception()
        {
            var product = new Product(Guid.NewGuid(), "Product #1", 100M, ProductKind.Pizza);
            var expectedException = new OrderCannotBeNullException();

            var exception = Record.Exception(() => product.AddOrders(null));

            exception.ShouldNotBeNull();
            exception.ShouldBeOfType(expectedException.GetType());
            exception.Message.ShouldBe(expectedException.Message);
        }

        [Fact]
        public void given_valid_order_should_add()
        {
            var product = new Product(Guid.NewGuid(), "Product #1", 100M, ProductKind.MainDish);
            var order = new Order(Guid.NewGuid(), "ORDER", DateTime.UtcNow, 100M, Email.Of("email@email.com"));

            product.AddOrder(order);

            product.Orders.ShouldNotBeEmpty();
            product.Orders.Count().ShouldBe(1);
        }

        [Fact]
        public void given_null_order_when_add_should_throw_an_exception()
        {
            var product = new Product(Guid.NewGuid(), "Product #1", 100M, ProductKind.MainDish);
            var expectedException = new OrderCannotBeNullException();

            var exception = Record.Exception(() => product.AddOrder(null));

            exception.ShouldNotBeNull();
            exception.ShouldBeOfType(expectedException.GetType());
            exception.Message.ShouldBe(expectedException.Message);
        }

        [Fact]
        public void given_existed_order_when_add_should_throw_an_exception()
        {
            var product = new Product(Guid.NewGuid(), "Product #1", 100M, ProductKind.MainDish);
            var order = new Order(Guid.NewGuid(), "ORDER", DateTime.UtcNow, 100M, Email.Of("email@email.com"));
            product.AddOrder(order);
            var expectedException = new OrderAlreadyExistsException(order.Id);

            var exception = Record.Exception(() => product.AddOrder(order));

            exception.ShouldNotBeNull();
            exception.ShouldBeOfType(expectedException.GetType());
            exception.Message.ShouldBe(expectedException.Message);
            ((OrderAlreadyExistsException)exception).OrderId.ShouldBe(expectedException.OrderId);
        }
    }
}
