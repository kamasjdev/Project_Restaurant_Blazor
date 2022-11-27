using Microsoft.Extensions.Logging;
using Restaurant.Core.Entities;
using Restaurant.Core.Repositories;
using Restaurant.Core.ValueObjects;
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
            var command = _dbConnection.CreateCommand();
            command.CommandText = sql;
            command.CommandType = CommandType.Text;
            command.AddParameter("@Id", user.Id.Value);
            command.AddParameter("@Email", user.Email.Value);
            command.AddParameter("@Password", user.Password);
            command.AddParameter("@Role", user.Role);
            command.AddParameter("@CreatedAt", user.CreatedAt);
            _logger.LogInformation($"Infrastructure: Invoking query: {sql}");
            return command.ExecuteScalarAsync();
        }

        public Task DeleteAsync(User user)
        {
            var sql = "DELETE FROM users WHERE Id = @Id";
            var command = _dbConnection.CreateCommand();
            command.CommandText = sql;
            command.CommandType = CommandType.Text;
            command.AddParameter("@Id", user.Id.Value);
            _logger.LogInformation($"Infrastructure: Invoking query: {sql}");
            return command.ExecuteScalarAsync();
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            var sql = "SELECT Id, Email, Password, Role, CreatedAt FROM users";
            var command = _dbConnection.CreateCommand();
            command.CommandText = sql;
            command.CommandType = CommandType.Text;
            _logger.LogInformation($"Infrastructure: Invoking query: {sql}");
            using var reader = await command.ExecuteReaderAsync();

            if (!reader.HasRows)
            {
                return new List<User>();
            }

            var list = new List<User>();
            while (reader.Read())
            {
                list.Add(new User(reader.GetGuid("Id"), 
                    Email.Of(reader.GetString("Email")), 
                    reader.GetString("Password"), 
                    reader.GetString("Role"), 
                    reader.GetDateTime("CreatedAt")));
            }

            return list;
        }

        public async Task<User?> GetAsync(Guid id)
        {
            var sql = "SELECT Id, Email, Password, Role, CreatedAt FROM users WHERE Id = @Id";
            var command = _dbConnection.CreateCommand();
            command.CommandText = sql;
            command.CommandType = CommandType.Text;
            command.AddParameter("@Id", id);
            _logger.LogInformation($"Infrastructure: Invoking query: {sql}");
            using var reader = await command.ExecuteReaderAsync();
            User? user = null;

            if (!reader.HasRows)
            {
                return user;
            }

            while (reader.Read())
            {
                user = new User(reader.GetGuid("Id"),
                    Email.Of(reader.GetString("Email")),
                    reader.GetString("Password"),
                    reader.GetString("Role"),
                    reader.GetDateTime("CreatedAt"));
            }

            return user;
        }

        public async Task<User?> GetAsync(string email)
        {
            var sql = "SELECT Id, Email, Password, Role, CreatedAt FROM users WHERE Email = @Email";
            var command = _dbConnection.CreateCommand();
            command.CommandText = sql;
            command.CommandType = CommandType.Text;
            command.AddParameter("@Email", email);
            _logger.LogInformation($"Infrastructure: Invoking query: {sql}");
            using var reader = await command.ExecuteReaderAsync();
            User? user = null;

            if (!reader.HasRows)
            {
                return user;
            }

            while (reader.Read())
            {
                user = new User(reader.GetGuid("Id"),
                    Email.Of(reader.GetString("Email")),
                    reader.GetString("Password"),
                    reader.GetString("Role"),
                    reader.GetDateTime("CreatedAt"));
            }

            return user;
        }

        public Task UpdateAsync(User user)
        {
            var sql = "UPDATE users SET Email = @Email, Password = @Password, Role = @Role, CreatedAt = @CreatedAt WHERE Id = @Id";
            var command = _dbConnection.CreateCommand();
            command.CommandText = sql;
            command.CommandType = CommandType.Text;
            command.AddParameter("@Id", user.Id.Value);
            command.AddParameter("@Email", user.Email.Value);
            command.AddParameter("@Password", user.Password);
            command.AddParameter("@Role", user.Role);
            command.AddParameter("@CreatedAt", user.CreatedAt);
            _logger.LogInformation($"Infrastructure: Invoking query: {sql}");
            return command.ExecuteScalarAsync();
        }
    }
}
