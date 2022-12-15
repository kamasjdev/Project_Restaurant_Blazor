using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using Grpc.Core;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute.ExceptionExtensions;
using Restaurant.UI.DTO;
using Restaurant.UI.Pages.Products;
using Restaurant.UI.Services.Abstractions;
using Restaurant.UI.UnitTests.Common;
using System.ComponentModel;
using System.Security.Claims;

namespace Restaurant.UI.UnitTests.Pages.Products
{
    public class ProductsPageTests
    {
        [Fact]
        public void should_render_products_page()
        {
            _component = _testContext.RenderComponent<ProductsPage>();
            _component.Markup.ShouldNotBeNullOrWhiteSpace();
        }

        [Fact]
        public void should_render_products_page_with_table_body()
        {
            _component = _testContext.RenderComponent<ProductsPage>();
            var tbody = _component.Find("table tbody");
            tbody.ShouldNotBeNull();
            tbody.InnerHtml.ShouldNotBeNullOrWhiteSpace();
        }

        [Fact]
        public void should_render_products_page_with_loading_icon()
        {
            _component = _testContext.RenderComponent<ProductsPage>();
            _component.Instance.Loading = true;
            _component.Render();

            var spinner = _component.Find(".spinner-border");

            spinner.ShouldNotBeNull();
        }

        [Fact]
        public void given_error_when_getting_data_from_api_should_show_error()
        {
            _component = _testContext.RenderComponent<ProductsPage>();
            _productService.GetAllAsync().ThrowsForAnyArgs(new Exception());
            _component = _testContext.RenderComponent<ProductsPage>();
            var genericMessage = "Something bad happen";

            var error = _component.Find(".alert.alert-danger");

            error.ShouldNotBeNull();
            _component.Instance.Error.ShouldContain(genericMessage);
            error.InnerHtml.ShouldBe(_component.Instance.Error);
        }

        [Fact]
        public void should_open_modal()
        {
            Authorize();
            _component = _testContext.RenderComponent<ProductsPage>();
            var firstRow = _component.Find("table tbody tr");
            var id = GetIdFromTableRow(firstRow);
            id.ShouldNotBe(Guid.Empty);
            var buttonDelete = firstRow.QuerySelector(".btn.btn-danger");
            buttonDelete.ShouldNotBeNull();

            buttonDelete.Click();

            var product = _products.SingleOrDefault(p => p.Id == id);
            product.ShouldNotBeNull();
            _component.Markup.ShouldContain("Do you wish to delete product");
            var modalContent = _component.Find(".modal-content");
            modalContent.ShouldNotBeNull();
        }

        [Fact]
        public async Task should_open_modal_and_delete_product()
        {
            Authorize();
            _component = _testContext.RenderComponent<ProductsPage>();
            var firstRow = _component.Find("table tbody tr");
            var id = GetIdFromTableRow(firstRow);
            id.ShouldNotBe(Guid.Empty);
            var buttonDelete = firstRow.QuerySelector(".btn.btn-danger");
            buttonDelete.ShouldNotBeNull();
            buttonDelete.Click();
            var modalContent = _component.Find(".modal-content");
            modalContent.ShouldNotBeNull();
            var content = modalContent.InnerHtml;
            var footerModal = _component.Find(".modal-footer");
            footerModal.ShouldNotBeNull();
            var deleteProduct = footerModal.QuerySelector(".btn.btn-danger.me-2");

            deleteProduct.ShouldNotBeNull();
            await deleteProduct.ClickAsync(new MouseEventArgs());

            await _productService.Received(1).DeleteAsync(id);
            var product = _products.SingleOrDefault(p => p.Id == id);
            product.ShouldNotBeNull();
            content.ShouldContain($"{product.Id}");
            content.ShouldContain($"{product.ProductName}");
        }

        private void Authorize()
        {
            var authContext = _testContext.AddTestAuthorization();
            var userClaims = TestFixture.CreateDefaultUser(role: "admin");
            authContext.SetClaims(userClaims.ToArray());
            authContext.SetAuthenticationType("auth_test");
            authContext.SetRoles("admin");
            authContext.SetAuthorized(userClaims.SingleOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value ?? "test", AuthorizationState.Authorized);
        }

        private Guid GetIdFromTableRow(IElement tableRow)
        {
            var idElement = tableRow.FirstChild as IHtmlTableDataCellElement;
            return idElement is not null && idElement.InnerHtml is not null ? Guid.Parse(idElement.InnerHtml) : Guid.Empty;
        }

        private readonly TestContext _testContext;
        private readonly IAuthenticationService _authenticationService;
        private readonly IProductService _productService;
        private IRenderedComponent<ProductsPage> _component;
        private readonly IEnumerable<ProductDto> _products;

        public ProductsPageTests()
        {
            _testContext = new TestContext();
            _testContext.AddTestAuthorization();
            _authenticationService = Substitute.For<IAuthenticationService>();
            _testContext.Services.AddScoped(_ => _authenticationService);
            _productService = Substitute.For<IProductService>();
            _testContext.Services.AddScoped(_ => _productService);
            _products = TestFixture.GetProducts();
            _products.ToList().ForEach(p => _productService.GetAsync(p.Id).Returns(p));
            _productService.GetAllAsync().Returns(_products);
        }
    }
}
