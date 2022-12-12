using Restaurant.Core.Entities;

namespace Restaurant.Core.Repositories
{
    public interface IProductSaleRepository
    {
        Task AddAsync(ProductSale productSale);
        Task UpdateAsync(ProductSale productSale);
        Task DeleteAsync(ProductSale productSale);
        Task<ProductSale?> GetAsync(Guid id);
        Task<IEnumerable<ProductSale>> GetAllAsync();
        Task<IEnumerable<ProductSale>> GetAllByOrderIdAsync(Guid orderId);
        Task<IEnumerable<ProductSale>> GetAllInCartByEmailAsync(string email);
        Task DeleteByOrderAsync(Guid orderId);
    }
}
