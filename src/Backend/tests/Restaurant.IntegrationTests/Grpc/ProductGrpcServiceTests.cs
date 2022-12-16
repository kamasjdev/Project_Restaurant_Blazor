using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Restaurant.Application.Exceptions;
using Restaurant.Core.ValueObjects;
using Restaurant.IntegrationTests.Common;
using Restaurant.Shared.ProductProto;
using Shouldly;

namespace Restaurant.IntegrationTests.Grpc
{
    public class ProductGrpcServiceTests : GrpcTestBase
    {
        [Fact]
        public async Task should_add_product()
        {
            var product = new Product { ProductName = nameof(ProductName), ProductKind = "Pizza", Price = "120,25" };

            var response = await _client.AddProductAsync(product);

            response.ShouldNotBeNull();
            response.Id.ShouldNotBeNullOrWhiteSpace();
            var productAdded = await _client.GetProductAsync(new GetProductRequest { Id = response.Id });
            productAdded.ShouldNotBeNull();
            productAdded.ProductName.ShouldBe(product.ProductName);
            productAdded.ProductKind.ShouldBe(product.ProductKind);
            decimal.Parse(productAdded.Price).ShouldBe(decimal.Parse(product.Price));
        }

        [Fact]
        public async Task should_update_product()
        {
            var productAdded = await AddDefaultProductAsync();
            var product = new Product
            {
                Id = productAdded.Id.Value.ToString(),
                ProductName = "abcTest123",
                ProductKind = "MainDish",
                Price = "505,50"
            };

            await _client.UpdateProductAsync(product);

            var productUpdated = await _client.GetProductAsync(new GetProductRequest { Id = product.Id });
            productUpdated.ShouldNotBeNull();
            productUpdated.ProductName.ShouldBe(product.ProductName);
            productUpdated.ProductKind.ShouldBe(product.ProductKind);
            decimal.Parse(productUpdated.Price).ShouldBe(decimal.Parse(product.Price));
        }

        [Fact]
        public async Task should_delete_product()
        {
            var productAdded = await AddDefaultProductAsync();

            await _client.DeleteProductAsync(new DeleteProductRequest { Id = productAdded.Id.Value.ToString() });

            var expectedException = new ProductNotFoundException(productAdded.Id);
            var productNotExistsException = await Record.ExceptionAsync(() => _client.GetProductAsync(new GetProductRequest { Id = productAdded.Id.Value.ToString() }).ResponseAsync);
            productNotExistsException.ShouldNotBeNull();
            productNotExistsException.ShouldBeOfType<RpcException>();
            ((RpcException)productNotExistsException).StatusCode.ShouldBe(StatusCode.FailedPrecondition);
            ((RpcException)productNotExistsException).Status.StatusCode.ShouldBe(StatusCode.FailedPrecondition);
            ((RpcException)productNotExistsException).Status.Detail.ShouldNotBeNullOrWhiteSpace();
            ((RpcException)productNotExistsException).Status.Detail.ShouldBe(expectedException.Message);
        }

        [Fact]
        public async Task should_get_all_addition()
        {
            var productAdded1 = await AddDefaultProductAsync();
            var productAdded2 = await AddDefaultProductAsync();

            var products = await _client.GetProductsAsync(new Empty());

            products.ShouldNotBeNull();
            products.Products.ShouldNotBeNull();
            products.Products.ShouldNotBeEmpty();
            products.Products.Count.ShouldBeGreaterThan(1);
            products.Products.ShouldContain(a => a.Id.Equals(productAdded1.Id.Value.ToString(), StringComparison.InvariantCultureIgnoreCase));
            products.Products.ShouldContain(a => a.Id.Equals(productAdded2.Id.Value.ToString(), StringComparison.InvariantCultureIgnoreCase));
        }

        [Fact]
        public async Task given_not_existing_product_should_return_exception()
        {
            var id = Guid.NewGuid();
            var expectedException = new ProductNotFoundException(id);

            var exception = await Record.ExceptionAsync(() => _client.GetProductAsync(new GetProductRequest { Id = id.ToString() }).ResponseAsync);

            exception.ShouldNotBeNull();
            exception.ShouldBeOfType<RpcException>();
            ((RpcException)exception).StatusCode.ShouldBe(StatusCode.FailedPrecondition);
            ((RpcException)exception).Status.StatusCode.ShouldBe(StatusCode.FailedPrecondition);
            ((RpcException)exception).Status.Detail.ShouldNotBeNullOrWhiteSpace();
            ((RpcException)exception).Status.Detail.ShouldBe(expectedException.Message);
        }

        private readonly Products.ProductsClient _client;

        public ProductGrpcServiceTests(OptionsProvider optionsProvider)
            : base(optionsProvider)
        {
            _client = new Products.ProductsClient(Channel);
        }
    }
}
