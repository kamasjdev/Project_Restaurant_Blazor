using Grpc.Core;
using Restaurant.IntegrationTests.Common;
using Restaurant.Shared.AdditionProto;
using Restaurant.Shared.OrderProto;
using Restaurant.Shared.ProductProto;
using Restaurant.Shared.ProductSaleProto;
using Restaurant.Shared.UserProto;
using Shouldly;

namespace Restaurant.IntegrationTests.Grpc
{
    public class ValidationGrpcEndpointsTests : GrpcTestBase
    {
        [Fact]
        public async Task given_invalid_id_when_get_addition_should_throw_an_exception()
        {
            var exception = await Record.ExceptionAsync(() => _additionsClient.GetAdditionAsync(new GetAdditionRequest
            {
                Id = "id"
            }).ResponseAsync);

            exception.ShouldNotBeNull();
            exception.ShouldBeOfType<RpcException>();
            ((RpcException)exception).StatusCode.ShouldBe(StatusCode.InvalidArgument);
            ((RpcException)exception).Status.Detail.ShouldNotBeNullOrWhiteSpace();
        }

        [Fact]
        public async Task given_invalid_request_when_add_addition_should_throw_an_exception()
        {
            var exception = await Record.ExceptionAsync(() => _additionsClient.AddAdditionAsync(new Shared.AdditionProto.Addition
            {
                AdditionName = "addition",
                AdditionKind = "kind",
                Price = "am"
            }).ResponseAsync);

            exception.ShouldNotBeNull();
            exception.ShouldBeOfType<RpcException>();
            ((RpcException)exception).StatusCode.ShouldBe(StatusCode.InvalidArgument);
            ((RpcException)exception).Status.Detail.ShouldNotBeNullOrWhiteSpace();
        }

        [Fact]
        public async Task given_invalid_request_when_update_addition_should_throw_an_exception()
        {
            var exception = await Record.ExceptionAsync(() => _additionsClient.UpdateAdditionAsync(new Shared.AdditionProto.Addition
            {
                Id = "test",
                AdditionName = "addition",
                AdditionKind = "kind",
                Price = "test"
            }).ResponseAsync);

            exception.ShouldNotBeNull();
            exception.ShouldBeOfType<RpcException>();
            ((RpcException)exception).StatusCode.ShouldBe(StatusCode.InvalidArgument);
            ((RpcException)exception).Status.Detail.ShouldNotBeNullOrWhiteSpace();
        }

        [Fact]
        public async Task given_invalid_id_when_delete_addition_should_throw_an_exception()
        {
            var exception = await Record.ExceptionAsync(() => _additionsClient.DeleteAdditionAsync(new DeleteAdditionRequest
            {
                Id = "id"
            }).ResponseAsync);

            exception.ShouldNotBeNull();
            exception.ShouldBeOfType<RpcException>();
            ((RpcException)exception).StatusCode.ShouldBe(StatusCode.InvalidArgument);
            ((RpcException)exception).Status.Detail.ShouldNotBeNullOrWhiteSpace();
        }

        [Fact]
        public async Task given_invalid_id_when_get_product_should_throw_an_exception()
        {
            var exception = await Record.ExceptionAsync(() => _productsClient.GetProductAsync(new GetProductRequest
            {
                Id = "id"
            }).ResponseAsync);

            exception.ShouldNotBeNull();
            exception.ShouldBeOfType<RpcException>();
            ((RpcException)exception).StatusCode.ShouldBe(StatusCode.InvalidArgument);
            ((RpcException)exception).Status.Detail.ShouldNotBeNullOrWhiteSpace();
        }

        [Fact]
        public async Task given_invalid_request_when_add_product_should_throw_an_exception()
        {
            var exception = await Record.ExceptionAsync(() => _productsClient.AddProductAsync(new Shared.ProductProto.Product
            {
                ProductName = "product",
                ProductKind = "kind",
                Price = "am"
            }).ResponseAsync);

            exception.ShouldNotBeNull();
            exception.ShouldBeOfType<RpcException>();
            ((RpcException)exception).StatusCode.ShouldBe(StatusCode.InvalidArgument);
            ((RpcException)exception).Status.Detail.ShouldNotBeNullOrWhiteSpace();
        }

        [Fact]
        public async Task given_invalid_request_when_update_product_should_throw_an_exception()
        {
            var exception = await Record.ExceptionAsync(() => _productsClient.UpdateProductAsync(new Shared.ProductProto.Product
            {
                Id = "test",
                ProductName = "product",
                ProductKind = "kind",
                Price = "test"
            }).ResponseAsync);

            exception.ShouldNotBeNull();
            exception.ShouldBeOfType<RpcException>();
            ((RpcException)exception).StatusCode.ShouldBe(StatusCode.InvalidArgument);
            ((RpcException)exception).Status.Detail.ShouldNotBeNullOrWhiteSpace();
        }

        [Fact]
        public async Task given_invalid_id_when_delete_product_should_throw_an_exception()
        {
            var exception = await Record.ExceptionAsync(() => _productsClient.DeleteProductAsync(new DeleteProductRequest
            {
                Id = "id"
            }).ResponseAsync);

            exception.ShouldNotBeNull();
            exception.ShouldBeOfType<RpcException>();
            ((RpcException)exception).StatusCode.ShouldBe(StatusCode.InvalidArgument);
            ((RpcException)exception).Status.Detail.ShouldNotBeNullOrWhiteSpace();
        }

        [Fact]
        public async Task given_invalid_id_when_get_product_sale_should_throw_an_exception()
        {
            var exception = await Record.ExceptionAsync(() => _productSalesClient.GetProductSaleAsync(new GetProductSaleRequest
            {
                Id = "id"
            }).ResponseAsync);

            exception.ShouldNotBeNull();
            exception.ShouldBeOfType<RpcException>();
            ((RpcException)exception).StatusCode.ShouldBe(StatusCode.InvalidArgument);
            ((RpcException)exception).Status.Detail.ShouldNotBeNullOrWhiteSpace();
        }

        [Fact]
        public async Task given_invalid_request_when_add_product_sale_should_throw_an_exception()
        {
            var exception = await Record.ExceptionAsync(() => _productSalesClient.AddProductAsync(new AddProductSaleRequest
            {
                Email = "email",
                OrderId = "ord",
                ProductId = "prod",
                AdditionId= "id"
            }).ResponseAsync);

            exception.ShouldNotBeNull();
            exception.ShouldBeOfType<RpcException>();
            ((RpcException)exception).StatusCode.ShouldBe(StatusCode.InvalidArgument);
            ((RpcException)exception).Status.Detail.ShouldNotBeNullOrWhiteSpace();
        }

        [Fact]
        public async Task given_invalid_request_when_update_product_sale_should_throw_an_exception()
        {
            var exception = await Record.ExceptionAsync(() => _productSalesClient.UpdateProductAsync(new AddProductSaleRequest
            {
                Email = "email",
                OrderId = "ord",
                ProductId = "prod",
                AdditionId = "id"
            }).ResponseAsync);

            exception.ShouldNotBeNull();
            exception.ShouldBeOfType<RpcException>();
            ((RpcException)exception).StatusCode.ShouldBe(StatusCode.InvalidArgument);
            ((RpcException)exception).Status.Detail.ShouldNotBeNullOrWhiteSpace();
        }

        [Fact]
        public async Task given_invalid_id_when_delete_product_sale_should_throw_an_exception()
        {
            var exception = await Record.ExceptionAsync(() => _productSalesClient.DeleteProductSaleAsync(new DeleteProductSaleRequest
            {
                Id = "id"
            }).ResponseAsync);

            exception.ShouldNotBeNull();
            exception.ShouldBeOfType<RpcException>();
            ((RpcException)exception).StatusCode.ShouldBe(StatusCode.InvalidArgument);
            ((RpcException)exception).Status.Detail.ShouldNotBeNullOrWhiteSpace();
        }

        [Fact]
        public async Task given_invalid_id_when_get_order_should_throw_an_exception()
        {
            var exception = await Record.ExceptionAsync(() => _ordersClient.GetOrderAsync(new GetOrderRequest
            {
                Id = "id"
            }).ResponseAsync);

            exception.ShouldNotBeNull();
            exception.ShouldBeOfType<RpcException>();
            ((RpcException)exception).StatusCode.ShouldBe(StatusCode.InvalidArgument);
            ((RpcException)exception).Status.Detail.ShouldNotBeNullOrWhiteSpace();
        }

        [Fact]
        public async Task given_invalid_request_when_add_order_should_throw_an_exception()
        {
            var request = new AddOrderRequest
            {
                Email = "email",
                Note = "note",
            };
            request.ProductSaleIds.Add(new ProductSaleId { Id = "abcv" });
            var exception = await Record.ExceptionAsync(() => _ordersClient.AddOrderAsync(request).ResponseAsync);

            exception.ShouldNotBeNull();
            exception.ShouldBeOfType<RpcException>();
            ((RpcException)exception).StatusCode.ShouldBe(StatusCode.InvalidArgument);
            ((RpcException)exception).Status.Detail.ShouldNotBeNullOrWhiteSpace();
        }

        [Fact]
        public async Task given_invalid_request_when_update_order_should_throw_an_exception()
        {
            var request = new AddOrderRequest
            {
                Id = "id",
                Email = "email",
                Note = "note",
            };
            request.ProductSaleIds.Add(new ProductSaleId { Id = "abcv" });
            var exception = await Record.ExceptionAsync(() => _ordersClient.UpdateOrderAsync(request).ResponseAsync);

            exception.ShouldNotBeNull();
            exception.ShouldBeOfType<RpcException>();
            ((RpcException)exception).StatusCode.ShouldBe(StatusCode.InvalidArgument);
            ((RpcException)exception).Status.Detail.ShouldNotBeNullOrWhiteSpace();
        }

        [Fact]
        public async Task given_invalid_id_when_delete_order_should_throw_an_exception()
        {
            var exception = await Record.ExceptionAsync(() => _ordersClient.DeleteOrderAsync(new DeleteOrderRequest
            {
                Id = "id"
            }).ResponseAsync);

            exception.ShouldNotBeNull();
            exception.ShouldBeOfType<RpcException>();
            ((RpcException)exception).StatusCode.ShouldBe(StatusCode.InvalidArgument);
            ((RpcException)exception).Status.Detail.ShouldNotBeNullOrWhiteSpace();
        }

        [Fact]
        public async Task given_invalid_id_when_get_user_should_throw_an_exception()
        {
            var exception = await Record.ExceptionAsync(() => _usersClient.GetUserAsync(new GetUserRequest
            {
                Id = "id"
            }).ResponseAsync);

            exception.ShouldNotBeNull();
            exception.ShouldBeOfType<RpcException>();
            ((RpcException)exception).StatusCode.ShouldBe(StatusCode.InvalidArgument);
            ((RpcException)exception).Status.Detail.ShouldNotBeNullOrWhiteSpace();
        }

        [Fact]
        public async Task given_invalid_request_when_update_user_should_throw_an_exception()
        {
            var exception = await Record.ExceptionAsync(() => _usersClient.UpdateUserAsync(new UpdateUserRequest
            {
                Email = "email",
                Password = "pass",
                Role = "role",
                UserId = "id"
            }).ResponseAsync);

            exception.ShouldNotBeNull();
            exception.ShouldBeOfType<RpcException>();
            ((RpcException)exception).StatusCode.ShouldBe(StatusCode.InvalidArgument);
            ((RpcException)exception).Status.Detail.ShouldNotBeNullOrWhiteSpace();
        }

        [Fact]
        public async Task given_invalid_id_when_change_user_email_should_throw_an_exception()
        {
            var exception = await Record.ExceptionAsync(() => _usersClient.ChangeUserEmailAsync(new ChangeUserEmailRequest
            {
                UserId = "id",
                Email = "email"
            }).ResponseAsync);

            exception.ShouldNotBeNull();
            exception.ShouldBeOfType<RpcException>();
            ((RpcException)exception).StatusCode.ShouldBe(StatusCode.InvalidArgument);
            ((RpcException)exception).Status.Detail.ShouldNotBeNullOrWhiteSpace();
        }

        [Fact]
        public async Task given_invalid_id_when_change_user_password_should_throw_an_exception()
        {
            var exception = await Record.ExceptionAsync(() => _usersClient.ChangeUserPasswordAsync(new ChangeUserPasswordRequest
            {
                UserId = "id",
                NewPassword = "password",
                NewPasswordConfirm = "password",
                Password = "password"
            }).ResponseAsync);

            exception.ShouldNotBeNull();
            exception.ShouldBeOfType<RpcException>();
            ((RpcException)exception).StatusCode.ShouldBe(StatusCode.InvalidArgument);
            ((RpcException)exception).Status.Detail.ShouldNotBeNullOrWhiteSpace();
        }

        [Fact]
        public async Task given_invalid_id_when_update_user_role_should_throw_an_exception()
        {
            var exception = await Record.ExceptionAsync(() => _usersClient.UpdateUserRoleAsync(new UpdateUserRoleRequest
            {
                UserId = "id",
                Role = "test"
            }).ResponseAsync);

            exception.ShouldNotBeNull();
            exception.ShouldBeOfType<RpcException>();
            ((RpcException)exception).StatusCode.ShouldBe(StatusCode.InvalidArgument);
            ((RpcException)exception).Status.Detail.ShouldNotBeNullOrWhiteSpace();
        }

        [Fact]
        public async Task given_invalid_id_when_delete_user_should_throw_an_exception()
        {
            var exception = await Record.ExceptionAsync(() => _usersClient.DeleteUserAsync(new DeleteUserRequest
            {
                UserId = "id"
            }).ResponseAsync);

            exception.ShouldNotBeNull();
            exception.ShouldBeOfType<RpcException>();
            ((RpcException)exception).StatusCode.ShouldBe(StatusCode.InvalidArgument);
            ((RpcException)exception).Status.Detail.ShouldNotBeNullOrWhiteSpace();
        }

        [Fact]
        public async Task given_not_authorized_user_when_get_user_should_throw_an_exception()
        {
            SetAnnonymous();
            var exception = await Record.ExceptionAsync(() => _usersClient.DeleteUserAsync(new DeleteUserRequest
            {
                UserId = "id"
            }).ResponseAsync);

            exception.ShouldNotBeNull();
            exception.ShouldBeOfType<RpcException>();
            ((RpcException)exception).StatusCode.ShouldBe(StatusCode.Unauthenticated);
            ((RpcException)exception).Status.StatusCode.ShouldBe(StatusCode.Unauthenticated);
            ((RpcException)exception).Status.Detail.ShouldNotBeNullOrWhiteSpace();
        }

        [Fact]
        public async Task given_not_authorized_user_when_add_product_should_throw_an_exception()
        {
            SetAnnonymous();
            var exception = await Record.ExceptionAsync(() => _productsClient.AddProductAsync(new Shared.ProductProto.Product
            {
                ProductName = "name",
                ProductKind = "kind",
                Price = "120"
            }).ResponseAsync);

            exception.ShouldNotBeNull();
            exception.ShouldBeOfType<RpcException>();
            ((RpcException)exception).StatusCode.ShouldBe(StatusCode.Unauthenticated);
            ((RpcException)exception).Status.StatusCode.ShouldBe(StatusCode.Unauthenticated);
            ((RpcException)exception).Status.Detail.ShouldNotBeNullOrWhiteSpace();
        }

        [Fact]
        public async Task given_not_authorized_user_when_update_addition_should_throw_an_exception()
        {
            SetAnnonymous();
            var exception = await Record.ExceptionAsync(() => _additionsClient.AddAdditionAsync(new Shared.AdditionProto.Addition
            {
                AdditionName = "name",
                AdditionKind = "kind",
                Price = "90"
            }).ResponseAsync);

            exception.ShouldNotBeNull();
            exception.ShouldBeOfType<RpcException>();
            ((RpcException)exception).StatusCode.ShouldBe(StatusCode.Unauthenticated);
            ((RpcException)exception).Status.StatusCode.ShouldBe(StatusCode.Unauthenticated);
            ((RpcException)exception).Status.Detail.ShouldNotBeNullOrWhiteSpace();
        }

        [Fact]
        public async Task given_not_authorized_user_when_get_product_sale_should_throw_an_exception()
        {
            SetAnnonymous();
            var exception = await Record.ExceptionAsync(() => _productSalesClient.GetProductSaleAsync(new GetProductSaleRequest
            {
                Id = Guid.NewGuid().ToString()
            }).ResponseAsync);

            exception.ShouldNotBeNull();
            exception.ShouldBeOfType<RpcException>();
            ((RpcException)exception).StatusCode.ShouldBe(StatusCode.Unauthenticated);
            ((RpcException)exception).Status.StatusCode.ShouldBe(StatusCode.Unauthenticated);
            ((RpcException)exception).Status.Detail.ShouldNotBeNullOrWhiteSpace();
        }

        [Fact]
        public async Task given_not_authorized_user_when_delete_order_should_throw_an_exception()
        {
            SetAnnonymous();
            var exception = await Record.ExceptionAsync(() => _ordersClient.DeleteOrderAsync(new DeleteOrderRequest
            {
                Id = Guid.NewGuid().ToString()
            }).ResponseAsync);

            exception.ShouldNotBeNull();
            exception.ShouldBeOfType<RpcException>();
            ((RpcException)exception).StatusCode.ShouldBe(StatusCode.Unauthenticated);
            ((RpcException)exception).Status.StatusCode.ShouldBe(StatusCode.Unauthenticated);
            ((RpcException)exception).Status.Detail.ShouldNotBeNullOrWhiteSpace();
        }

        private readonly Additions.AdditionsClient _additionsClient;
        private readonly Products.ProductsClient _productsClient;
        private readonly ProductSales.ProductSalesClient _productSalesClient;
        private readonly Orders.OrdersClient _ordersClient;
        private readonly Users.UsersClient _usersClient;

        public ValidationGrpcEndpointsTests(OptionsProvider optionsProvider)
            : base(optionsProvider)
        {
            _additionsClient = new Additions.AdditionsClient(Channel);
            _productsClient = new Products.ProductsClient(Channel);
            _productSalesClient = new ProductSales.ProductSalesClient(Channel);
            _ordersClient = new Orders.OrdersClient(Channel);
            _usersClient = new Users.UsersClient(Channel);
        }
    }
}
