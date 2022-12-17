using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Restaurant.Application.Exceptions;
using Restaurant.Core.ValueObjects;
using Restaurant.IntegrationTests.Common;
using Restaurant.Shared.OrderProto;
using Restaurant.Shared.ProductProto;
using Restaurant.Shared.ProductSaleProto;
using Shouldly;

namespace Restaurant.IntegrationTests.Grpc
{
    public class OrderGrpcServiceTests : GrpcTestBase
    {
        [Fact]
        public async Task should_add_order()
        {
            var productSale = await AddDefaultProductSale();
            var request = new AddOrderRequest
            {
                Email = _email,
                Note = "this is note"
            };
            request.ProductSaleIds.Add(new ProductSaleId { Id = productSale.Id.Value.ToString() });

            var response = await _client.AddOrderAsync(request);

            response.ShouldNotBeNull();
            var order = await _client.GetOrderAsync(new GetOrderRequest { Id = response.Id });
            order.ShouldNotBeNull();
            order.Email.ShouldBe(request.Email);
            order.Note.ShouldBe(request.Note);
            order.Products.ShouldContain(p => p.Id == productSale.Id.Value.ToString());
        }

        [Fact]
        public async Task should_update_order()
        {
            var productSale1 = await AddDefaultProductSale();
            var productSale2 = await AddDefaultProductSale();
            var order = await AddDefaultOrder();
            var request = new AddOrderRequest
            {
                Id = order.Id.Value.ToString(),
                Email = _email,
                Note = "abc note test"
            };
            request.ProductSaleIds.AddRange(new List<ProductSaleId> { new ProductSaleId { Id = productSale1.Id.Value.ToString() }, new ProductSaleId { Id = productSale2.Id.Value.ToString() } });

            await _client.UpdateOrderAsync(request);

            var orderUpdated = await _client.GetOrderAsync(new GetOrderRequest { Id = order.Id.Value.ToString() });
            orderUpdated.ShouldNotBeNull();
            orderUpdated.Email.ShouldBe(request.Email);
            orderUpdated.Note.ShouldBe(request.Note);
            orderUpdated.Products.ShouldContain(ps => ps.Id == productSale1.Id.Value.ToString());
            orderUpdated.Products.ShouldContain(ps => ps.Id == productSale2.Id.Value.ToString());
            orderUpdated.Products.ShouldNotContain(ps => ps.Id == order.Products.First().Id.Value.ToString());
        }

        [Fact]
        public async Task should_delete_order()
        {
            var order = await AddDefaultOrder();
            var productSalesIds = order.Products.Select(ps => ps.Id).ToList();

            await _client.DeleteOrderAsync(new DeleteOrderRequest { Id = order.Id.Value.ToString() });

            var exception = await Record.ExceptionAsync(() => _client.GetOrderAsync(new GetOrderRequest { Id = order.Id.Value.ToString() }).ResponseAsync);
            var expectedException = new OrderNotFoundException(order.Id);
            exception.ShouldNotBeNull();
            exception.ShouldBeOfType<RpcException>();
            ((RpcException)exception).StatusCode.ShouldBe(StatusCode.FailedPrecondition);
            ((RpcException)exception).Status.StatusCode.ShouldBe(StatusCode.FailedPrecondition);
            ((RpcException)exception).Status.Detail.ShouldNotBeNullOrWhiteSpace();
            ((RpcException)exception).Status.Detail.ShouldBe(expectedException.Message);
            productSalesIds.ForEach(async (ps) =>
            {
                var productSale = await _productSalesClient.GetProductSaleAsync(new GetProductSaleRequest { Id = ps.Value.ToString() });
                productSale.ShouldNotBeNull();
            });
        }

        [Fact]
        public async Task should_delete_order_with_positions()
        {
            var order = await AddDefaultOrder();
            var productSalesIds = order.Products.Select(ps => ps.Id).ToList();

            await _client.DeleteOrderWithPositionsAsync(new DeleteOrderWithPositionsRequest { Id = order.Id.Value.ToString() });

            var exception = await Record.ExceptionAsync(() => _client.GetOrderAsync(new GetOrderRequest { Id = order.Id.Value.ToString() }).ResponseAsync);
            var expectedException = new OrderNotFoundException(order.Id);
            exception.ShouldNotBeNull();
            exception.ShouldBeOfType<RpcException>();
            ((RpcException)exception).StatusCode.ShouldBe(StatusCode.FailedPrecondition);
            ((RpcException)exception).Status.StatusCode.ShouldBe(StatusCode.FailedPrecondition);
            ((RpcException)exception).Status.Detail.ShouldNotBeNullOrWhiteSpace();
            ((RpcException)exception).Status.Detail.ShouldBe(expectedException.Message);
            productSalesIds.ForEach(async (ps) =>
            {
                var expectedExceptionProductSale = new ProductNotFoundException(ps);
                var exceptionProductSale = await Record.ExceptionAsync(() => _productSalesClient.GetProductSaleAsync(new GetProductSaleRequest { Id = ps.Value.ToString() }).ResponseAsync);
                exceptionProductSale.ShouldNotBeNull();
                exceptionProductSale.ShouldBeOfType<RpcException>();
                ((RpcException)exceptionProductSale).StatusCode.ShouldBe(StatusCode.FailedPrecondition);
                ((RpcException)exceptionProductSale).Status.StatusCode.ShouldBe(StatusCode.FailedPrecondition);
                ((RpcException)exceptionProductSale).Status.Detail.ShouldNotBeNullOrWhiteSpace();
                ((RpcException)exceptionProductSale).Status.Detail.ShouldBe(expectedExceptionProductSale.Message);
            });

        }

        [Fact]
        public async Task should_get_all_orders()
        {
            var order1 = await AddDefaultOrder();
            var order2 = await AddDefaultOrder();

            var orders = await _client.GetOrdersAsync(new Empty());

            orders.ShouldNotBeNull();
            orders.Orders.ShouldNotBeEmpty();
            orders.Orders.Count.ShouldBeGreaterThan(1);
            orders.Orders.ShouldContain(o => o.Id == order1.Id.Value.ToString());
            orders.Orders.ShouldContain(o => o.Id == order2.Id.Value.ToString());
        }

        private async Task<Core.Entities.Order> AddDefaultOrder()
        {
            var productSale = await AddDefaultProductSale();
            return await AddDefaultOrderAsync(Email.Of(_email), new List<Core.Entities.ProductSale> { productSale });
        }

        private async Task<Core.Entities.ProductSale> AddDefaultProductSale()
        {
            var product = await AddDefaultProductAsync();
            var addition = await AddDefaultAdditionAsync();
            return await AddDefaultProductSaleAsync(product.Id, product.Price.Value + addition.Price.Value, Email.Of("email@email.com"), addition.Id);
        }

        private readonly Orders.OrdersClient _client;
        private readonly ProductSales.ProductSalesClient _productSalesClient;
        private const string _email = "email@email.com";

        public OrderGrpcServiceTests(OptionsProvider optionsProvider)
            : base(optionsProvider)
        {
            _client = new Orders.OrdersClient(Channel);
            _productSalesClient = new ProductSales.ProductSalesClient(Channel);
        }
    }
}
