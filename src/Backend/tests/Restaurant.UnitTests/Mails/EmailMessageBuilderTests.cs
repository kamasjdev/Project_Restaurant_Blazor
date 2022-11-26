using Restaurant.Application.Exceptions;
using Restaurant.Core.Entities;
using Restaurant.Core.ValueObjects;
using Shouldly;
using static Restaurant.Application.Mail.EmailMessage;

namespace Restaurant.UnitTests.Mails
{
    public class EmailMessageBuilderTests
    {
        [Fact]
        public void should_create_mail_from_order()
        {
            var order = CreateDefaultOrder();

            var mail = _emailMessageBuilder.ConstructEmailFromOrder(order);

            mail.Subject.ShouldBe($"Zamówienie nr {order.OrderNumber.Value}");
            mail.Body.ShouldContain(order.Price.Value.ToString());
        }

        [Fact]
        public void should_create_mail_from_order_with_positions()
        {
            var order = CreateDefaultOrderWithPositions();

            var mail = _emailMessageBuilder.ConstructEmailFromOrder(order);

            mail.Subject.ShouldBe($"Zamówienie nr {order.OrderNumber.Value}");
            mail.Body.ShouldContain(order.Price.Value.ToString());
            foreach (var product in order.Products)
            {
                mail.Body.ShouldContain(product.Product.ProductName.Value);
            }
        }

        [Fact]
        public void should_create_mail_from_order_with_product_and_addition()
        {
            var order = CreateDefaultOrder();
            var note = "Notes1234";
            order.ChangeNote(note);
            var productSale = CreateDefaultProductSale();
            productSale.ChangeAddition(CreateDefaultAddition());

            var mail = _emailMessageBuilder.ConstructEmailFromOrder(order);

            mail.Subject.ShouldBe($"Zamówienie nr {order.OrderNumber.Value}");
            mail.Body.ShouldContain(order.Price.Value.ToString());
            mail.Body.ShouldContain(note);
            foreach (var product in order.Products)
            {
                mail.Body.ShouldContain(product.Product.ProductName.Value);
                mail.Body.ShouldContain(product.Addition.AdditionName.Value);
            }
        }

        [Fact]
        public void given_null_order_should_throw_an_exception()
        {
            var expectedException = new CannotConstructEmailFromOrderException();

            var exception = Record.Exception(() => _emailMessageBuilder.ConstructEmailFromOrder(null));

            exception.ShouldNotBeNull();
            exception.ShouldBeOfType(expectedException.GetType());
            exception.Message.ShouldBe(expectedException.Message);
        }

        private Order CreateDefaultOrder()
        {
            var order = new Order(Guid.NewGuid(), "ORDER", _currentDate, 0, Email.Of("email@email.com"));
            return order;
        }

        private Order CreateDefaultOrderWithPositions()
        {
            var productSales = new List<ProductSale> { CreateDefaultProductSale(), CreateDefaultProductSale() };
            var order = new Order(Guid.NewGuid(), "ORDER", _currentDate, productSales.Sum(p => p.EndPrice), Email.Of("email@email.com"), products: productSales);
            return order;
        }

        private ProductSale CreateDefaultProductSale()
        {
            var product = CreateDefaultProduct();
            var productSale = new ProductSale(Guid.NewGuid(), product, ProductSaleState.Ordered, Email.Of("email@email.com"));
            return productSale;
        }

        private Product CreateDefaultProduct()
        {
            var product = new Product(Guid.NewGuid(), Guid.NewGuid().ToString("N"), 10, ProductKind.MainDish);
            return product;
        }

        private Addition CreateDefaultAddition()
        {
            var addition = new Addition(Guid.NewGuid(), Guid.NewGuid().ToString("N"), 10, AdditionKind.Drink);
            return addition;
        }

        private readonly DateTime _currentDate;
        private readonly EmailMessageBuilder _emailMessageBuilder;

        public EmailMessageBuilderTests()
        {
            _currentDate = new DateTime(2022, 9, 13, 18, 20, 30);
            _emailMessageBuilder = new EmailMessageBuilder();
        }
    }
}
