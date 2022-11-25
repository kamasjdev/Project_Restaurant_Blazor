using Restaurant.Core.Entities;
using Restaurant.Core.Repositories;

namespace Restaurant.Infrastructure.Repositories.InMemory
{
    internal sealed class InMemoryOrderRepository : IOrderRepository
    {
        private readonly List<Order> _orders = new();

        public Task AddAsync(Order order)
        {
            _orders.Add(order);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Order order)
        {
            _orders.Remove(order);
            return Task.CompletedTask;
        }

        public async Task<IEnumerable<Order>> GetAllAsync()
        {
            await Task.CompletedTask;
            return _orders;
        }

        public async Task<Order?> GetAsync(Guid id)
        {
            await Task.CompletedTask;
            return _orders.SingleOrDefault(o => o.Id == id);
        }

        public Task UpdateAsync(Order order)
        {
            return Task.CompletedTask;
        }
    }
}
