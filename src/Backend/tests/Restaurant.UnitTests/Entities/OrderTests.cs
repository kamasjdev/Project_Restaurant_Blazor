using Restaurant.Core.Entities;
using Restaurant.Core.Exceptions;
using Restaurant.Core.ValueObjects;
using Shouldly;

namespace Restaurant.UnitTests.Entities
{
    public class OrderTests
    {
        [Fact]
        public void should_create_order()
        {
            var id = Guid.NewGuid();
            var orderNumber = "ORDER/2022";
            var createDate = DateTime.UtcNow;
            var price = 200;
            var email = Email.Of("email@email.com");

            var order = new Order(id, orderNumber, createDate, price, email);

            order.ShouldNotBeNull();
            order.Id.Value.ShouldBe(id);
            order.OrderNumber.Value.ShouldBe(orderNumber);
            order.Created.ShouldBe(createDate);
            order.Price.Value.ShouldBe(price);
            order.Email.ShouldBe(email);
        }

        [Fact]
        public void should_add_products()
        {
            var order = CreateDefaultOrder();
            var productSale = CreateDefaultProductSale();

            order.AddProducts(new List<ProductSale> { productSale });

            order.Products.Count().ShouldBe(1);
            var productAdded = order.Products.First();
            productAdded.Id.ShouldBe(productSale.Id);
        }

        [Fact]
        public void given_null_products_when_add_should_throw_an_exception()
        {
            var order = CreateDefaultOrder();
            var expectedException = new ProductCannotBeNullException();

            var exception = Record.Exception(() => order.AddProducts(null));

            exception.ShouldNotBeNull();
            exception.ShouldBeOfType(expectedException.GetType());
            exception.Message.ShouldBe(expectedException.Message);
        }

        [Fact]
        public void should_add_product()
        {
            var order = CreateDefaultOrder();
            var productSale = CreateDefaultProductSale();

            order.AddProduct(productSale);

            order.Products.Count().ShouldBe(1);
            var productAdded = order.Products.First();
            productAdded.Id.ShouldBe(productSale.Id);
        }

        [Fact]
        public void given_existed_product_when_add_should_throw_an_exception()
        {
            var order = CreateDefaultOrder();
            var productSale = CreateDefaultProductSale();
            order.AddProduct(productSale);
            var expectedException = new ProductAlreadyExistsException(productSale.Id);

            var exception = Record.Exception(() => order.AddProduct(productSale));

            exception.ShouldNotBeNull();
            exception.ShouldBeOfType(expectedException.GetType());
            exception.Message.ShouldBe(expectedException.Message);
            ((ProductAlreadyExistsException)exception).ProductId.ShouldBe(expectedException.ProductId);
        }

        [Fact]
        public void given_null_product_when_add_should_throw_an_exception()
        {
            var order = CreateDefaultOrder();
            var expectedException = new ProductCannotBeNullException();

            var exception = Record.Exception(() => order.AddProduct(null));

            exception.ShouldNotBeNull();
            exception.ShouldBeOfType(expectedException.GetType());
            exception.Message.ShouldBe(expectedException.Message);
        }

        [Fact]
        public void should_remove_product_from_order()
        {
            var order = CreateDefaultOrder();
            var productSale = CreateDefaultProductSale();
            order.AddProduct(productSale);

            order.RemoveProduct(productSale);

            order.Products.Count().ShouldBe(0);
        }

        [Fact]
        public void given_not_existed_product_should_throw_an_exception()
        {
            var order = CreateDefaultOrder();
            var productSale = CreateDefaultProductSale();
            var expectedException = new ProductNotFoundException(productSale.Id);

            var exception = Record.Exception(() => order.RemoveProduct(productSale));

            exception.ShouldNotBeNull();
            exception.ShouldBeOfType(expectedException.GetType());
            exception.Message.ShouldBe(expectedException.Message);
            ((ProductNotFoundException)exception).ProductId.ShouldBe(expectedException.ProductId);
        }

        [Fact]
        public void given_null_product_when_remove_should_throw_an_exception()
        {
            var order = CreateDefaultOrder();
            var expectedException = new ProductCannotBeNullException();

            var exception = Record.Exception(() => order.RemoveProduct(null));

            exception.ShouldNotBeNull();
            exception.ShouldBeOfType(expectedException.GetType());
            exception.Message.ShouldBe(expectedException.Message);
        }

        private Order CreateDefaultOrder()
        {
            return new Order(Guid.NewGuid(), "ORDER/2022", DateTime.UtcNow, 100, Email.Of("email@test.pl"));
        }

        private ProductSale CreateDefaultProductSale()
        {
            return new ProductSale(Guid.NewGuid(), CreateDefaultProduct(), ProductSaleState.New, Email.Of("test@test.com"));
        }

        private Product CreateDefaultProduct()
        {
            return new Product(Guid.NewGuid(), "Product#1", 100, ProductKind.Pizza);
        }
    }
}
