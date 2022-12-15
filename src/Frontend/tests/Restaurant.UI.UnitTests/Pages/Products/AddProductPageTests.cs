using Grpc.Core;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute.ExceptionExtensions;
using Restaurant.UI.DTO;
using Restaurant.UI.Pages.Products;
using Restaurant.UI.Services.Abstractions;

namespace Restaurant.UI.UnitTests.Pages.Products
{
    public class AddProductPageTests
    {
        [Fact]
        public void should_render_add_product_page()
        {
            _component.Markup.ShouldContain("AddProductPage");
        }

        [Fact]
        public void given_invalid_product_name_when_send_request_should_show_error()
        {
            var instance = _component.Instance;
            var productNameInput = _component.Find("#ProductName");
            var productName = "Product";
            productNameInput.Change(productName);
            var priceInput = _component.Find("#Price");
            priceInput.Change("100");
            var sendButton = _component.Find(".btn.btn-success.me-2");
            var expectedException = new RpcException(new Status(StatusCode.FailedPrecondition, "Invalid ProductName"));
            _productService.AddAsync(Arg.Is<ProductDto>(p => p.ProductName == productName))
                .ThrowsForAnyArgs(expectedException);

            sendButton.Click();

            var error = _component.Find(".alert.alert-danger");
            error.ShouldNotBeNull();
            instance.Error.ShouldNotBeNull();
            instance.Error.ShouldBe(expectedException.Status.Detail);
            error.InnerHtml.ShouldContain(instance.Error);
        }

        private readonly TestContext _testContext;
        private readonly IAuthenticationService _authenticationService;
        private readonly IProductService _productService;
        private readonly IRenderedComponent<AddProductPage> _component;

        public AddProductPageTests()
        {
            _testContext = new TestContext();
            _testContext.AddTestAuthorization();
            _authenticationService = Substitute.For<IAuthenticationService>();
            _testContext.Services.AddScoped(_ => _authenticationService);
            _productService = Substitute.For<IProductService>();
            _testContext.Services.AddScoped(_ => _productService);
            _component = _testContext.RenderComponent<AddProductPage>();
        }
    }
}
