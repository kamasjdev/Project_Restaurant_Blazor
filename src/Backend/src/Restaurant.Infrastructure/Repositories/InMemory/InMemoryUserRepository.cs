using Restaurant.Core.Entities;
using Restaurant.Core.Repositories;

namespace Restaurant.Infrastructure.Repositories.InMemory
{
    internal sealed class InMemoryUserRepository : IUserRepository
    {
        private readonly List<User> _users = new();

        public Task AddAsync(User user)
        {
            _users.Add(user);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(User user)
        {
            _users.Remove(user);
            return Task.CompletedTask;
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            await Task.CompletedTask;
            return _users;
        }

        public async Task<User?> GetAsync(Guid id)
        {
            await Task.CompletedTask;
            return _users.SingleOrDefault(u => u.Id == id);
        }

        public async Task<User?> GetAsync(string email)
        {
            await Task.CompletedTask;
            return _users.SingleOrDefault(u => u.Email.Value == email);
        }

        public Task UpdateAsync(User user)
        {
            return Task.CompletedTask;
        }
    }
}
