using Org.BouncyCastle.Asn1.X509;
using Restaurant.Core.Entities;
using Restaurant.Core.Repositories;
using Restaurant.Core.ValueObjects;
using Restaurant.IntegrationTests.Common;
using Shouldly;

namespace Restaurant.IntegrationTests.RepositoryTests
{
    public class ProductSaleRepositoryTests : BaseTest
    {
        [Fact]
        public async Task should_add_product_sale()
        {
            var product = await AddDefaultProductAsync();
            var productSale = new ProductSale(Guid.NewGuid(), product, ProductSaleState.New, Email.Of("email@email.com"));

            await _productSaleRepository.AddAsync(productSale);

            var productSaleAdded = await _productSaleRepository.GetAsync(productSale.Id);
            productSaleAdded.ShouldNotBeNull();
            productSaleAdded.Email.ShouldBe(productSale.Email);
            productSaleAdded.EndPrice.ShouldBe(productSale.EndPrice);
            productSaleAdded.ProductSaleState.ShouldBe(productSale.ProductSaleState);
        }

        [Fact]
        public async Task should_return_product_sale_with_addition_and_order()
        {
            var product = await AddDefaultProductAsync();
            var addition = await AddDefaultAdditionAsync();
            var productSale = await AddDefaultProductSaleAsync(product.Id, product.Price.Value + addition.Price.Value, Email.Of("email@test.com"), addition.Id);
            var order = await AddDefaultOrderAsync(productSale.Email, new List<ProductSale> { productSale });

            var productSaleAdded = await _productSaleRepository.GetAsync(productSale.Id);

            productSaleAdded.ShouldNotBeNull();
            productSaleAdded.ShouldNotBeNull();
            productSaleAdded.Email.ShouldBe(productSale.Email);
            productSaleAdded.EndPrice.ShouldBe(productSale.EndPrice);
            productSaleAdded.ProductSaleState.ShouldBe(productSale.ProductSaleState);
            productSaleAdded.Product.ShouldNotBeNull();
            productSaleAdded.Product.Id.ShouldBe(product.Id);
            productSaleAdded.Addition.ShouldNotBeNull();
            productSaleAdded.Addition.Id.ShouldBe(addition.Id);
            productSaleAdded.Order.ShouldNotBeNull();
            productSaleAdded.Order.Id.ShouldBe(order.Id);
        }

        [Fact]
        public async Task should_return_all_product_sales()
        {
            var product = await AddDefaultProductAsync();
            var productSale = await AddDefaultProductSaleAsync(product.Id, product.Price.Value, Email.Of("email@test.com"));
            var productSale2 = await AddDefaultProductSaleAsync(product.Id, product.Price.Value, Email.Of("email@test.com"));

            var productSales = await _productSaleRepository.GetAllAsync();

            productSales.ShouldNotBeEmpty();
            productSales.Count().ShouldBeGreaterThan(1);
            productSales.ShouldContain(p => p.Id.Equals(productSale.Id));
            productSales.ShouldContain(p => p.Id.Equals(productSale2.Id));
        }

        [Fact]
        public async Task should_update_product_sale()
        {
            var product = await AddDefaultProductAsync();
            var productSale = await AddDefaultProductSaleAsync(product.Id, product.Price.Value, Email.Of("email@test.com"));
            var addition = await AddDefaultAdditionAsync();
            productSale.ChangeAddition(addition);
            productSale.ChangeEmail(Email.Of("psaleemail@email.com"));

            await _productSaleRepository.UpdateAsync(productSale);

            var productSaleUpdated = await _productSaleRepository.GetAsync(productSale.Id);
            productSaleUpdated.ShouldNotBeNull();
            productSaleUpdated.Addition.ShouldNotBeNull();
            productSaleUpdated.Addition.Id.ShouldBe(addition.Id);
            productSaleUpdated.AdditionId.ShouldBe(addition.Id);
            productSaleUpdated.Email.ShouldBe(productSale.Email);
        }

        [Fact]
        public async Task should_delete_product_sale()
        {
            var product = await AddDefaultProductAsync();
            var productSale = await AddDefaultProductSaleAsync(product.Id, product.Price.Value, Email.Of("email@test.com"));

            await _productSaleRepository.DeleteAsync(productSale);

            var productDeleted = await _productSaleRepository.GetAsync(productSale.Id);
            productDeleted.ShouldBeNull();
        }

        [Fact]
        public async Task should_get_all_product_sales_by_order_id()
        {
            var product = await AddDefaultProductAsync();
            var productSale = await AddDefaultProductSaleAsync(product.Id, product.Price.Value, Email.Of("email@test.com"));
            var productSale2 = await AddDefaultProductSaleAsync(product.Id, product.Price.Value, Email.Of("email@test.com"));
            var order = await AddDefaultOrderAsync(productSale.Email, new List<ProductSale> { productSale, productSale2 });

            var productSales = await _productSaleRepository.GetAllByOrderIdAsync(order.Id);

            productSales.ShouldNotBeEmpty();
            productSales.Count().ShouldBeGreaterThan(1);
            productSales.ShouldContain(ps => ps.Id.Equals(productSale.Id));
            productSales.ShouldContain(ps => ps.Id.Equals(productSale2.Id));
        }

        [Fact]
        public async Task should_get_all_product_sales_by_email()
        {
            var product = await AddDefaultProductAsync();
            var productSale = await AddDefaultProductSaleAsync(product.Id, product.Price.Value, Email.Of("email@test.com"));
            var productSale2 = await AddDefaultProductSaleAsync(product.Id, product.Price.Value, Email.Of("email@test.com"));

            var productSales = await _productSaleRepository.GetAllByEmailAsync(productSale.Email.Value);

            productSales.ShouldNotBeEmpty();
            productSales.Count().ShouldBeGreaterThan(1);
            productSales.ShouldContain(ps => ps.Id.Equals(productSale.Id));
            productSales.ShouldContain(ps => ps.Id.Equals(productSale2.Id));
        }

        private readonly IProductSaleRepository _productSaleRepository;

        public ProductSaleRepositoryTests(OptionsProvider optionsProvider) 
            : base(optionsProvider)
        {
            _productSaleRepository = GetRequiredService<IProductSaleRepository>();
        }
    }
}
