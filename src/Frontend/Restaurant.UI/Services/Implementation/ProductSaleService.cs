using Restaurant.Shared.ProductSaleProto;
using Restaurant.UI.DTO;
using Restaurant.UI.Services.Abstractions;

namespace Restaurant.UI.Services.Implementation
{
    internal sealed class ProductSaleService : IProductSaleService
    {
        private readonly ProductSales.ProductSalesClient _productSalesClient;

        public ProductSaleService(ProductSales.ProductSalesClient productSalesClient)
        {
            _productSalesClient = productSalesClient;
        }

        public async Task<Guid> AddAsync(ProductSaleDto productSaleDto)
        {
            var request = new AddProductSaleRequest
            {
                Id = productSaleDto.Id.ToString(),
                Email = productSaleDto.Email,
                ProductId = productSaleDto.Product.Id.ToString(),
            };

            if (productSaleDto.Addition is not null)
            {
                request.AdditionId = productSaleDto.Addition.Id.ToString();
            }

            if (productSaleDto.Order is not null)
            {
                request.OrderId = productSaleDto.Order.Id.ToString();
            }

            var response = await _productSalesClient.AddProductAsync(request);
            return Guid.Parse(response.Id);
        }

        public async Task DeleteAsync(Guid id)
        {
            await _productSalesClient.DeleteProductSaleAsync(new DeleteProductSaleRequest
            {
                Id = id.ToString()
            });
        }

        public async Task<IEnumerable<ProductSaleDto>> GetAllInCartByEmailAsync(string email)
        {
            return (await _productSalesClient.GetProductSalesByEmailAsync(new GetProductSalesByEmailRequest { Email = email }))
                .ProductSales.Select(ps => new ProductSaleDto
                {
                    Id = Guid.Parse(ps.Id),
                    Email = ps.Email,
                    Addition = ps.Addition is not null ? new AdditionDto
                    {
                        Id = Guid.Parse(ps.Addition.Id),
                        AdditionKind = ps.Addition.AdditionKind,
                        AdditionName = ps.Addition.AdditionName,
                        Price = decimal.Parse(ps.Addition.Price)
                    } : null,
                    Product = new ProductDto
                    {
                        Id = Guid.Parse(ps.Product.Id),
                        Price = decimal.Parse(ps.Product.Price),
                        ProductKind = ps.Product.ProductKind,
                        ProductName = ps.Product.ProductName
                    },
                    EndPrice = decimal.Parse(ps.EndPrice),
                    ProductSaleState = ps.ProductSaleState
                });
        }

        public async Task<ProductSaleDto?> GetAsync(Guid productSaleId)
        {
            var productSale = await _productSalesClient.GetProductSaleAsync(new GetProductSaleRequest
            {
                Id = productSaleId.ToString()
            });
            return productSale is not null ? new ProductSaleDto
            {
                Id = Guid.Parse(productSale.Id),
                Email = productSale.Email,
                Addition = productSale.Addition is not null ? new AdditionDto
                {
                    Id = Guid.Parse(productSale.Addition.Id),
                    AdditionName = productSale.Addition.AdditionName,
                    AdditionKind = productSale.Addition.AdditionKind,
                    Price = decimal.Parse(productSale.Addition.Price)
                } : null,
                Product = new ProductDto
                {
                    Id = Guid.Parse(productSale.Product.Id),
                    Price = decimal.Parse(productSale.Product.Price),
                    ProductKind = productSale.Product.ProductKind,
                    ProductName = productSale.Product.ProductName
                },
                EndPrice = decimal.Parse(productSale.EndPrice),
                ProductSaleState = productSale.ProductSaleState,
                Order = productSale.Order is not null ? new OrderDto
                {
                    Id = Guid.Parse(productSale.Order.Id),
                    Email = productSale.Order.Email,
                    Note = productSale.Order.Note,
                    OrderNumber = productSale.Order.OrderNumber,
                    Price = decimal.Parse(productSale.Order.Price),
                    Created = productSale.Order.Created.ToDateTime()
                } : null
            } : null;
        }
    }
}
