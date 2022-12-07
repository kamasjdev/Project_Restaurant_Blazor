using Restaurant.UI.DTO;
using Restaurant.UI.Services.Abstractions;

namespace Restaurant.UI.Services.Implementation
{
    public class ProductSaleService : IProductSaleService
    {
        private readonly List<ProductSaleDto> _productSales = new();

        public Task<Guid> AddAsync(ProductSaleDto productSaleDto)
        {
            productSaleDto.Id = Guid.NewGuid();
            _productSales.Add(productSaleDto);
            return Task.FromResult(productSaleDto.Id);
        }

        public Task DeleteAsync(Guid id)
        {
            var productSale = _productSales.SingleOrDefault(p => p.Id == id);

            if (productSale is null)
            {
                return Task.CompletedTask;
            }

            _productSales.Remove(productSale);
            return Task.CompletedTask;
        }
    }
}
