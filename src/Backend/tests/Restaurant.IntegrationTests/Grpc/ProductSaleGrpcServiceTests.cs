using Grpc.Core;
using Restaurant.Application.Exceptions;
using Restaurant.Core.ValueObjects;
using Restaurant.IntegrationTests.Common;
using Restaurant.Shared.OrderProto;
using Restaurant.Shared.ProductSaleProto;
using Shouldly;

namespace Restaurant.IntegrationTests.Grpc
{
    public class ProductSaleGrpcServiceTests : GrpcTestBase
    {
        [Fact]
        public async Task should_add_product_sale()
        {
            var product = await AddDefaultProductAsync();
            var addition = await AddDefaultAdditionAsync();

            var response = await _client.AddProductAsync(new AddProductSaleRequest { ProductId = product.Id.Value.ToString(), Email = "email@email.com", AdditionId = addition.Id.Value.ToString() });

            response.ShouldNotBeNull();
            var productSaleAdded = await _client.GetProductSaleAsync(new GetProductSaleRequest { Id = response.Id });
            productSaleAdded.ShouldNotBeNull();
            productSaleAdded.Product.ShouldNotBeNull();
            productSaleAdded.Product.Id.ShouldBe(product.Id.Value.ToString());
            productSaleAdded.Addition.ShouldNotBeNull();
            productSaleAdded.Addition.Id.ShouldBe(addition.Id.Value.ToString());

        }

        [Fact]
        public async Task should_add_product_sale_to_order()
        {
            var productSale = await AddDefaultProductSale();
            var order = await AddDefaultOrderAsync(productSale.Email, new List<Core.Entities.ProductSale> { productSale });

            var response = await _client.AddProductAsync(new AddProductSaleRequest { ProductId = productSale.ProductId.Value.ToString(), Email = productSale.Email.Value, OrderId = order.Id.Value.ToString() });

            response.ShouldNotBeNull();
            var productSaleAdded = await _client.GetProductSaleAsync(new GetProductSaleRequest { Id = response.Id });
            productSaleAdded.ShouldNotBeNull();
            productSaleAdded.Product.ShouldNotBeNull();
            productSaleAdded.Product.Id.ShouldBe(productSale.ProductId.Value.ToString());
            productSaleAdded.Order.ShouldNotBeNull();
            productSaleAdded.Order.Id.ShouldBe(order.Id.Value.ToString());
        }

        [Fact]
        public async Task should_delete_product_sale()
        {
            var productSale = await AddDefaultProductSale();

            await _client.DeleteProductSaleAsync(new DeleteProductSaleRequest { Id = productSale.Id.Value.ToString() });

            var exception = await Record.ExceptionAsync(() => _client.GetProductSaleAsync(new GetProductSaleRequest { Id = productSale.Id.Value.ToString() }).ResponseAsync);
            var expectedException = new ProductSaleNotFoundException(productSale.Id);
            exception.ShouldNotBeNull();
            exception.ShouldBeOfType<RpcException>();
            ((RpcException)exception).StatusCode.ShouldBe(StatusCode.FailedPrecondition);
            ((RpcException)exception).Status.StatusCode.ShouldBe(StatusCode.FailedPrecondition);
            ((RpcException)exception).Status.Detail.ShouldBe(expectedException.Message);
        }
        

        [Fact]
        public async Task should_delete_product_sale_from_order()
        {
            var productSale = await AddDefaultProductSale();
            var order = await AddDefaultOrderAsync(productSale.Email, new List<Core.Entities.ProductSale> { productSale });

            await _client.DeleteProductSaleAsync(new DeleteProductSaleRequest { Id = productSale.Id.Value.ToString() });

            var exception = await Record.ExceptionAsync(() => _client.GetProductSaleAsync(new GetProductSaleRequest { Id = productSale.Id.Value.ToString() }).ResponseAsync);
            var expectedException = new ProductSaleNotFoundException(productSale.Id);
            exception.ShouldNotBeNull();
            exception.ShouldBeOfType<RpcException>();
            ((RpcException)exception).StatusCode.ShouldBe(StatusCode.FailedPrecondition);
            ((RpcException)exception).Status.StatusCode.ShouldBe(StatusCode.FailedPrecondition);
            ((RpcException)exception).Status.Detail.ShouldBe(expectedException.Message);
            var orderModified = await _ordersClient.GetOrderAsync(new GetOrderRequest { Id = order.Id.Value.ToString() });
            orderModified.ShouldNotBeNull();
            orderModified.Products.ShouldNotContain(p => p.Id == productSale.Id.Value.ToString());
        }

        private async Task<Core.Entities.ProductSale> AddDefaultProductSale()
        {
            var product = await AddDefaultProductAsync();
            var addition = await AddDefaultAdditionAsync();
            return await AddDefaultProductSaleAsync(product.Id, product.Price.Value + addition.Price.Value, Email.Of("email@email.com"), addition.Id);
        }

        private readonly ProductSales.ProductSalesClient _client;
        private readonly Orders.OrdersClient _ordersClient;

        public ProductSaleGrpcServiceTests(OptionsProvider optionsProvider)
            : base(optionsProvider)
        {
            _client = new ProductSales.ProductSalesClient(Channel);
            _ordersClient = new Orders.OrdersClient(Channel);
        }
    }
}
