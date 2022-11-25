using Restaurant.Core.Entities;
using Restaurant.Core.Repositories;

namespace Restaurant.Infrastructure.Repositories.InMemory
{
    internal sealed class InMemoryProductSaleRepository : IProductSaleRepository
    {
        private readonly List<ProductSale> _productSales = new();

        public Task AddAsync(ProductSale productSale)
        {
            _productSales.Add(productSale);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(ProductSale productSale)
        {
            _productSales.Remove(productSale);
            return Task.CompletedTask;
        }

        public async Task<IEnumerable<ProductSale>> GetAllAsync()
        {
            await Task.CompletedTask;
            return _productSales;
        }

        public async Task<IEnumerable<ProductSale>> GetAllByOrderIdAsync(Guid orderId)
        {
            await Task.CompletedTask;
            return _productSales.Where(p => p.OrderId is not null && p.OrderId == orderId);
        }

        public async Task<IEnumerable<ProductSale>> GetAllByEmailAsync(string email)
        {
            await Task.CompletedTask;
            return _productSales.Where(p => p.Email.Value == email);
        }

        public async Task<ProductSale?> GetAsync(Guid id)
        {
            await Task.CompletedTask;
            return _productSales.SingleOrDefault(p => p.Id == id);
        }

        public Task UpdateAsync(ProductSale productSale)
        {
            return Task.CompletedTask;
        }
    }
}
