using Restaurant.Core.Entities;
using Restaurant.Core.Repositories;
using Restaurant.Core.ValueObjects;
using Restaurant.IntegrationTests.Common;
using Shouldly;

namespace Restaurant.IntegrationTests.RepositoryTests
{
    public class OrderRepositoryTests : BaseTest
    {
        [Fact]
        public async Task should_add_order()
        {
            var email = Email.Of("emailtestabc@user.com");
            var order = new Order(Guid.NewGuid(), "ORDERD1234", DateTime.UtcNow, 100M, email, "note#abc");

            await _orderRepository.AddAsync(order);

            var orderAdded = await _orderRepository.GetAsync(order.Id);
            orderAdded.ShouldNotBeNull();
            orderAdded.OrderNumber.ShouldBe(order.OrderNumber);
            orderAdded.Note.ShouldBe(order.Note);
            orderAdded.Price.ShouldBe(order.Price);
            orderAdded.Products.ShouldBeEmpty();
        }

        [Fact]
        public async Task should_update_order()
        {
            var email = Email.Of("emailtestabc@user.com");
            var product = await AddDefaultProductAsync();
            var productSale = await AddDefaultProductSaleAsync(product.Id, product.Price.Value, email);
            var order = new Order(Guid.NewGuid(), "ORDERD1234", DateTime.UtcNow, productSale.EndPrice, email, "note#abc", new List<ProductSale> { productSale });
            await _orderRepository.AddAsync(order);
            var notes = "Notest123412412";
            order.ChangeNote(notes);
            var orderNumber = "ORDER/2022/11/27";
            order.ChangeOrderNumber(orderNumber);
            await _productSaleRepository.UpdateAsync(productSale);

            await _orderRepository.UpdateAsync(order);

            var orderAdded = await _orderRepository.GetAsync(order.Id);
            orderAdded.ShouldNotBeNull();
            orderAdded.OrderNumber.Value.ShouldBe(orderNumber);
            orderAdded.Note.ShouldBe(notes);
            orderAdded.Price.ShouldBe(order.Price);
            orderAdded.Products.ShouldNotBeEmpty();
            orderAdded.Products.Count().ShouldBe(order.Products.Count());
            orderAdded.Products.First().Id.ShouldBe(productSale.Id);
        }

        [Fact]
        public async Task should_delete_order()
        {
            var email = Email.Of("email-test@abc.com");
            var order = await AddDefaultOrderAsync(email);

            await _orderRepository.DeleteAsync(order);

            var orderDeleted = await _orderRepository.GetAsync(order.Id);
            orderDeleted.ShouldBeNull();
        }

        [Fact]
        public async Task should_get_all_orders()
        {
            var email = Email.Of("emailtest3d@abc.com");
            var order = await AddDefaultOrderAsync(email);
            var order2 = await AddDefaultOrderAsync(email);

            var orders = await _orderRepository.GetAllAsync();

            orders.ShouldNotBeNull();
            orders.Count().ShouldBeGreaterThan(1);
            orders.ShouldContain(o => o.Id.Equals(order.Id));
            orders.ShouldContain(o => o.Id.Equals(order2.Id));
        }

        private readonly IOrderRepository _orderRepository;
        private readonly IProductSaleRepository _productSaleRepository;

        public OrderRepositoryTests(OptionsProvider optionsProvider)
            : base(optionsProvider)
        {
            _orderRepository = GetRequiredService<IOrderRepository>();
            _productSaleRepository = GetRequiredService<IProductSaleRepository>();
        }
    }
}
