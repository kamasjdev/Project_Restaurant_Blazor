using Restaurant.Core.Entities;

namespace Restaurant.Core.Repositories
{
    public interface IProductRepository
    {
        Task AddAsync(Product product);
        Task UpdateAsync(Product product);
        Task DeleteAsync(Product product);
        Task<Product?> GetAsync(Guid id);
        Task<IEnumerable<Product>> GetAllAsync();
    }
}
