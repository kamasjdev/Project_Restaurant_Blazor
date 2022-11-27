using Restaurant.Core.Entities;
using Restaurant.Core.Repositories;
using Restaurant.Core.ValueObjects;
using Restaurant.IntegrationTests.Common;
using Shouldly;

namespace Restaurant.IntegrationTests.RepositoryTests
{
    public class ProductRepositoryTests : BaseTest
    {
        [Fact]
        public async Task should_add_product()
        {
            var product = new Product(Guid.NewGuid(), "Product#1", 150M, ProductKind.Pizza);

            await _productRepository.AddAsync(product);

            var productAdded = await _productRepository.GetAsync(product.Id);
            productAdded.ShouldNotBeNull();
            productAdded.ProductName.ShouldBe(product.ProductName);
            productAdded.ProductKind.ShouldBe(product.ProductKind);
            productAdded.Price.ShouldBe(product.Price);
        }

        [Fact]
        public async Task should_get_product_with_orders()
        {
            var email = Email.Of("emial@abc.com");
            var product = await AddDefaultProductAsync();
            var addition = await AddDefaultAdditionAsync();
            var productSale = await AddDefaultProductSaleAsync(product.Id, product.Price.Value + addition.Price.Value, email, addition.Id);
            var productSale2 = await AddDefaultProductSaleAsync(product.Id, product.Price.Value + addition.Price.Value, email, addition.Id);
            var productSale3 = await AddDefaultProductSaleAsync(product.Id, product.Price.Value, email);
            await AddDefaultOrderAsync(email, new List<ProductSale> { productSale, productSale2 });
            await AddDefaultOrderAsync(email, new List<ProductSale> { productSale3 });

            var productAdded = await _productRepository.GetAsync(product.Id);

            productAdded.ShouldNotBeNull();
            productAdded.ProductSaleIds.ShouldNotBeEmpty();
            productAdded.ProductSaleIds.Count().ShouldBeGreaterThan(1);
            productAdded.Orders.ShouldNotBeEmpty();
            productAdded.Orders.Count().ShouldBeGreaterThan(1);
        }

        private readonly IProductRepository _productRepository;

        public ProductRepositoryTests(OptionsProvider optionsProvider)
            :base(optionsProvider)
        {
            _productRepository = GetRequiredService<IProductRepository>();
        }
    }
}
