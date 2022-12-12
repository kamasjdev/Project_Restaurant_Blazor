using Dapper;
using Microsoft.Extensions.Logging;
using Restaurant.Core.Entities;
using Restaurant.Core.Repositories;
using Restaurant.Core.ValueObjects;
using Restaurant.Infrastructure.Repositories.DBO;
using System.Data;
using System.Data.Common;

namespace Restaurant.Infrastructure.Repositories
{
    internal sealed class UserRepository : IUserRepository
    {
        private readonly DbConnection _dbConnection;
        private readonly ILogger<UserRepository> _logger;

        public UserRepository(DbConnection dbConnection, ILogger<UserRepository> logger)
        {
            _dbConnection = dbConnection;
            _logger = logger;
        }

        public Task AddAsync(User user)
        {
            var sql = "INSERT INTO users (Id, Email, Password, Role, CreatedAt) VALUES (@Id, @Email, @Password, @Role, @CreatedAt)";
            _logger.LogInformation($"Infrastructure: Invoking query: {sql}");
            return _dbConnection.ExecuteAsync(sql, new { Id = user.Id.Value, Email = user.Email.Value, user.Password,
                user.Role, user.CreatedAt });
        }

        public Task DeleteAsync(User user)
        {
            var sql = "DELETE FROM users WHERE Id = @Id";
            _logger.LogInformation($"Infrastructure: Invoking query: {sql}");
            return _dbConnection.ExecuteAsync(sql, new { Id = user.Id.Value });
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            var sql = "SELECT Id, Email, Password, Role, CreatedAt FROM users";
            _logger.LogInformation($"Infrastructure: Invoking query: {sql}");
            return (await _dbConnection.QueryAsync<UserDBO>(sql))
               .Select(u => new User(u.Id, Email.Of(u.Email), u.Password, u.Role, DateTime.SpecifyKind(u.CreatedAt, DateTimeKind.Utc)));
        }

        public async Task<User?> GetAsync(Guid id)
        {
            var sql = "SELECT Id, Email, Password, Role, CreatedAt FROM users WHERE Id = @Id";
            _logger.LogInformation($"Infrastructure: Invoking query: {sql}");
            var user = await _dbConnection.QuerySingleOrDefaultAsync<UserDBO>(sql, new { Id = id });
            return user is not null ? new User(user.Id, Email.Of(user.Email), user.Password, user.Role, DateTime.SpecifyKind(user.CreatedAt, DateTimeKind.Utc)) : null;
        }

        public async Task<User?> GetAsync(string email)
        {
            var sql = "SELECT Id, Email, Password, Role, CreatedAt FROM users WHERE Email = @Email";
            _logger.LogInformation($"Infrastructure: Invoking query: {sql}");
            var user = await _dbConnection.QuerySingleOrDefaultAsync<UserDBO>(sql, new { Email = email });
            return user is not null ? new User(user.Id, Email.Of(user.Email), user.Password, user.Role, DateTime.SpecifyKind(user.CreatedAt, DateTimeKind.Utc)) : null;
        }

        public Task UpdateAsync(User user)
        {
            var sql = "UPDATE users SET Email = @Email, Password = @Password, Role = @Role, CreatedAt = @CreatedAt WHERE Id = @Id";
            _logger.LogInformation($"Infrastructure: Invoking query: {sql}");
            return _dbConnection.ExecuteAsync(sql, new { Id = user.Id.Value, Email = user.Email.Value, user.Password,
                user.Role, user.CreatedAt });
        }
    }
}
