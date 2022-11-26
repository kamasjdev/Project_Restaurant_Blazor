using Restaurant.Core.Entities;
using Restaurant.Core.Exceptions;
using Restaurant.Core.ValueObjects;
using Shouldly;

namespace Restaurant.UnitTests.Entities
{
    public class ProductSaleTests
    {
        [Fact]
        public void should_create_product()
        {
            var id = Guid.NewGuid();
            var email = "email@test.com";
            var product = CreateDefaultProduct();

            var productSale = new ProductSale(id, product, ProductSaleState.New, Email.Of(email));

            productSale.ShouldNotBeNull();
            productSale.Id.Value.ShouldBe(id);
            productSale.Email.Value.ShouldBe(email);
            productSale.Product.Id.ShouldBe(product.Id);
            productSale.EndPrice.Value.ShouldBe(product.Price.Value);
        }

        [Fact]
        public void given_product_and_addition_should_create_product_with_proper_end_price()
        {
            var id = Guid.NewGuid();
            var email = "email@test.com";
            var product = CreateDefaultProduct();
            var addition = CreateDefaultAddition();

            var productSale = new ProductSale(id, product, ProductSaleState.New, Email.Of(email), addition);

            productSale.ShouldNotBeNull();
            productSale.Product.ShouldNotBeNull();
            productSale.ProductId.Value.ShouldBe(product.Id.Value);
            productSale.Addition.ShouldNotBeNull();
            productSale.Addition.Id.Value.ShouldBe(addition.Id.Value);
            productSale.EndPrice.Value.ShouldBe(product.Price.Value + addition.Price.Value);
        }

        [Fact]
        public void given_product_addition_and_order_should_create_product()
        {
            var id = Guid.NewGuid();
            var email = "email@test.com";
            var product = CreateDefaultProduct();
            var addition = CreateDefaultAddition();
            var order = CreateDefaultOrder();

            var productSale = new ProductSale(id, product, ProductSaleState.New, Email.Of(email), addition, order);

            productSale.ShouldNotBeNull();
            productSale.Product.ShouldNotBeNull();
            productSale.ProductId.Value.ShouldBe(product.Id.Value);
            productSale.Addition.ShouldNotBeNull();
            productSale.Addition.Id.Value.ShouldBe(addition.Id.Value);
            productSale.Order.ShouldNotBeNull();
            productSale.Order.Id.Value.ShouldBe(order.Id.Value);
        }

        [Fact]
        public void given_null_product_should_throw_an_exception()
        {
            var id = Guid.NewGuid();
            var email = "email@test.com";
            var expectedException = new ProductCannotBeNullException();

            var exception = Record.Exception(() => new ProductSale(id, product: null, ProductSaleState.New, Email.Of(email)));

            exception.ShouldNotBeNull();
            exception.ShouldBeOfType(expectedException.GetType());
            exception.Message.ShouldBe(expectedException.Message);
        }

        [Fact]
        public void given_valid_product_should_change()
        {
            var productSale = CreateDefaultProductSale();
            var product = new Product(Guid.NewGuid(), "Product#2022", 200, ProductKind.MainDish);

            productSale.ChangeProduct(product);

            productSale.Product.ShouldNotBeNull();
            productSale.Product.Id.Value.ShouldBe(product.Id.Value);
            productSale.Product.Price.Value.ShouldBe(product.Price.Value);
            productSale.Product.ProductName.Value.ShouldBe(product.ProductName.Value);
            productSale.Product.ProductKind.ShouldBe(product.ProductKind);
            productSale.EndPrice.Value.ShouldBe(product.Price.Value);
        }

        [Fact]
        public void given_null_when_change_product_should_throw_an_exception()
        {
            var productSale = CreateDefaultProductSale();
            var expectedException = new ProductCannotBeNullException();

            var exception = Record.Exception(() => productSale.ChangeProduct(null));

            exception.ShouldNotBeNull();
            exception.ShouldBeOfType(expectedException.GetType());
            exception.Message.ShouldBe(expectedException.Message);
        }

        [Fact]
        public void given_valid_addition_should_change()
        {
            var productSale = CreateDefaultProductSale();
            var addition = CreateDefaultAddition();
            var price = productSale.EndPrice;

            productSale.ChangeAddition(addition);

            productSale.Addition.ShouldNotBeNull();
            productSale.AdditionId.Value.ShouldBe(addition.Id.Value);
            productSale.EndPrice.Value.ShouldBe(price + addition.Price);
        }

        [Fact]
        public void given_null_addition_when_change_should_throw_an_exception()
        {
            var productSale = CreateDefaultProductSale();
            var expectedException = new AdditionCannotBeNullException();

            var exception = Record.Exception(() => productSale.ChangeAddition(null));

            exception.ShouldNotBeNull();
            exception.ShouldBeOfType(expectedException.GetType());
            exception.Message.ShouldBe(expectedException.Message);
        }

        [Fact]
        public void should_remove_addition()
        {
            var addition = CreateDefaultAddition();
            var productSale = new ProductSale(Guid.NewGuid(), CreateDefaultProduct(), ProductSaleState.New, Email.Of("email@email.com"), addition);

            productSale.RemoveAddition();

            productSale.Addition.ShouldBeNull();
            productSale.AdditionId.ShouldBeNull();
        }

        [Fact]
        public void given_product_sale_without_additon_when_remove_should_throw_an_exception()
        {
            var productSale = CreateDefaultProductSale();
            var expectedException = new AdditionNotExistsException(productSale.Id);

            var exception = Record.Exception(() => productSale.RemoveAddition());

            exception.ShouldNotBeNull();
            exception.ShouldBeOfType(expectedException.GetType());
            exception.Message.ShouldBe(expectedException.Message);
            ((AdditionNotExistsException)exception).ProductSaleId.ShouldBe(expectedException.ProductSaleId);
        }

        [Fact]
        public void should_add_order_to_product_sale()
        {
            var productSale = CreateDefaultProductSale();
            var order = CreateDefaultOrder();

            productSale.AddOrder(order);

            productSale.Order.ShouldNotBeNull();
            productSale.OrderId.ShouldNotBeNull();
            productSale.ProductSaleState.ShouldBe(ProductSaleState.Ordered);
        }

        [Fact]
        public void given_existing_order_when_add_should_throw_an_exception()
        {
            var productSale = CreateDefaultProductSale();
            var order = CreateDefaultOrder();
            productSale.AddOrder(order);
            var expectedException = new CannotOverrideExistingOrderException(productSale.Id);

            var exception = Record.Exception(() => productSale.AddOrder(order));

            exception.ShouldNotBeNull();
            exception.ShouldBeOfType(expectedException.GetType());
            exception.Message.ShouldBe(expectedException.Message);
            ((CannotOverrideExistingOrderException)exception).ProductSaleId.Value.ShouldBe(expectedException.ProductSaleId.Value);
        }

        [Fact]
        public void given_null_order_when_add_should_throw_an_exception()
        {
            var productSale = CreateDefaultProductSale();
            var expectedException = new OrderCannotBeNullException();

            var exception = Record.Exception(() => productSale.AddOrder(null));

            exception.ShouldNotBeNull();
            exception.ShouldBeOfType(expectedException.GetType());
            exception.Message.ShouldBe(expectedException.Message);
        }

        [Fact]
        public void should_remove_order_from_product_sale()
        {
            var productSale = CreateDefaultProductSale();
            var order = CreateDefaultOrder();
            productSale.AddOrder(order);

            productSale.RemoveOrder();

            productSale.Order.ShouldBeNull();
            productSale.OrderId.ShouldBeNull();
            productSale.ProductSaleState.ShouldBe(ProductSaleState.New);
        }

        [Fact]
        public void given_product_sale_without_order_when_remove_should_throw_an_exception()
        {
            var productSale = CreateDefaultProductSale();
            var expectedException = new OrderNotExsitsException(productSale.Id);

            var exception = Record.Exception(() => productSale.RemoveOrder());

            exception.ShouldNotBeNull();
            exception.ShouldBeOfType(expectedException.GetType());
            exception.Message.ShouldBe(expectedException.Message);
            ((OrderNotExsitsException)exception).ProductSaleId.ShouldBe(expectedException.ProductSaleId);
        }

        private ProductSale CreateDefaultProductSale()
        {
            return new ProductSale(Guid.NewGuid(), CreateDefaultProduct(), ProductSaleState.New, Email.Of("test@test.com"));
        }

        private Addition CreateDefaultAddition()
        {
            return new Addition(Guid.NewGuid(), "Addition#1", 10, AdditionKind.Salad);
        }

        private Product CreateDefaultProduct()
        {
            return new Product(Guid.NewGuid(), "Product#1", 100, ProductKind.Pizza);
        }

        private Order CreateDefaultOrder()
        {
            return new Order(Guid.NewGuid(), "Order#1", DateTime.UtcNow, 0, Email.Of("email@email.com"));
        }
    }
}