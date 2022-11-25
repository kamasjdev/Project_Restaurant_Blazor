using Restaurant.Core.Entities;
using Restaurant.Core.Repositories;

namespace Restaurant.Infrastructure.Repositories.InMemory
{
    internal sealed class InMemoryAdditionRepository : IAdditonRepository
    {
        private readonly List<Addition> _additions = new();

        public Task AddAsync(Addition addition)
        {
            _additions.Add(addition);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Addition addition)
        {
            _additions.Remove(addition);
            return Task.CompletedTask;
        }

        public async Task<IEnumerable<Addition>> GetAllAsync()
        {
            await Task.CompletedTask;
            return _additions;
        }

        public async Task<Addition?> GetAsync(Guid id)
        {
            await Task.CompletedTask;
            return _additions.SingleOrDefault(a => a.Id == id);
        }

        public Task UpdateAsync(Addition addition)
        {
            return Task.CompletedTask;
        }
    }
}
