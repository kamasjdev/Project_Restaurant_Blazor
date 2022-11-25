using Restaurant.Core.Entities;

namespace Restaurant.Core.Repositories
{
    public interface IOrderRepository
    {
        Task AddAsync(Order order);
        Task UpdateAsync(Order order);
        Task DeleteAsync(Order order);
        Task<Order?> GetAsync(Guid id);
        Task<IEnumerable<Order>> GetAllAsync();
    }
}
