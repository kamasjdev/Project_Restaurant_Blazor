using Restaurant.Core.Entities;
using Restaurant.Core.Repositories;

namespace Restaurant.Infrastructure.Repositories.InMemory
{
    internal sealed class InMemoryProductRepository : IProductRepository
    {
        private readonly List<Product> _products = new();

        public Task AddAsync(Product product)
        {
            _products.Add(product);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Product product)
        {
            _products.Remove(product);
            return Task.CompletedTask;
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            await Task.CompletedTask;
            return _products;
        }

        public async Task<Product?> GetAsync(Guid id)
        {
            await Task.CompletedTask;
            return _products.SingleOrDefault(p => p.Id == id);
        }

        public Task UpdateAsync(Product product)
        {
            return Task.CompletedTask;
        }
    }
}
