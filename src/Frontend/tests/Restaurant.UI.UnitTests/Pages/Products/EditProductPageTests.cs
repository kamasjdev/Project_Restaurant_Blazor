using Grpc.Core;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute.ExceptionExtensions;
using Restaurant.UI.DTO;
using Restaurant.UI.Pages.Products;
using Restaurant.UI.Services.Abstractions;
using Restaurant.UI.UnitTests.Common;

namespace Restaurant.UI.UnitTests.Pages.Products
{
    public class EditProductPageTests
    {
        [Fact]
        public void should_render_edit_product_page()
        {
            var product = _products.First();
            var component = _testContext.RenderComponent<EditProductPage>(ComponentParameter.CreateParameter("id", product.Id));
            
            component.ShouldNotBeNull();
            var errors = component.FindAll(".alert.alert-danger");
            errors.ShouldBeEmpty();
        }

        [Fact]
        public void given_invalid_id_should_show_error()
        {
            var id = Guid.NewGuid();
            var component = _testContext.RenderComponent<EditProductPage>(ComponentParameter.CreateParameter("id", id));

            var error = component.Find(".alert.alert-danger");

            error.ShouldNotBeNull();
            error.InnerHtml.ShouldNotBeNullOrWhiteSpace();
            error.InnerHtml.ShouldContain($"Product with id '{id}' was not found");
        }

        [Fact]
        public void given_invalid_price_when_send_request_should_show_error()
        {
            var product = _products.First();
            var component = _testContext.RenderComponent<EditProductPage>(ComponentParameter.CreateParameter("id", product.Id));
            var instance = component.Instance;
            var productNameInput = component.Find("#ProductName");
            productNameInput.Change("Product");
            var priceInput = component.Find("#Price");
            priceInput.Change("100");
            var sendButton = component.Find(".btn.btn-success.me-2");
            var expectedException = new RpcException(new Status(StatusCode.FailedPrecondition, "Invalid Price"));
            _productService.UpdateAsync(Arg.Any<ProductDto>())
                .ThrowsForAnyArgs(expectedException);

            sendButton.Click();

            var error = component.Find(".alert.alert-danger");
            error.ShouldNotBeNull();
            instance.Error.ShouldNotBeNull();
            instance.Error.ShouldBe(expectedException.Status.Detail);
            error.InnerHtml.ShouldContain(instance.Error);
        }

        [Fact]
        public void should_render_page_with_loading_icon()
        {
            var component = _testContext.RenderComponent<EditProductPage>();
            component.Instance.Loading = true;
            component.Render();

            var spinner = component.Find(".spinner-border");

            spinner.ShouldNotBeNull();
        }

        private readonly TestContext _testContext;
        private readonly IAuthenticationService _authenticationService;
        private readonly IProductService _productService;
        private readonly IEnumerable<ProductDto> _products;

        public EditProductPageTests()
        {
            _testContext = new TestContext();
            _testContext.AddTestAuthorization();
            _authenticationService = Substitute.For<IAuthenticationService>();
            _testContext.Services.AddScoped(_ => _authenticationService);
            _productService = Substitute.For<IProductService>();
            _testContext.Services.AddScoped(_ => _productService);
            _products = TestFixture.GetProducts();
            _productService.GetAllAsync().Returns(_products);
            _products.ToList().ForEach(p => _productService.GetAsync(p.Id).Returns(p));
        }
    }
}
