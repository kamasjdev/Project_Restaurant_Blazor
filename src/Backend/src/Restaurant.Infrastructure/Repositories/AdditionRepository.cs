using Restaurant.Core.Entities;
using Restaurant.Core.Repositories;
using System.Data;
using System.Data.Common;

namespace Restaurant.Infrastructure.Repositories
{
    internal sealed class AdditionRepository : IAdditonRepository
    {
        private readonly DbConnection _dbConnection;

        public AdditionRepository(DbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public Task AddAsync(Addition addition)
        {
            var sql = "INSERT INTO additions (Id, AdditionName, Price, AdditionKind) VALUES (@Id, @AdditionName, @Price, @AdditionKind)";
            var command = _dbConnection.CreateCommand();
            command.CommandText = sql;
            command.CommandType = CommandType.Text;
            command.AddParameter("@Id", addition.Id.Value);
            command.AddParameter("@AdditionName", addition.AdditionName.Value);
            command.AddParameter("@Price", addition.Price.Value);
            command.AddParameter("@AdditionKind", addition.AdditionKind.ToString());
            return command.ExecuteScalarAsync();
        }

        public Task DeleteAsync(Addition addition)
        {
            var sql = "DELETE FROM additions WHERE Id = @Id";
            var command = _dbConnection.CreateCommand();
            command.CommandText = sql;
            command.CommandType = CommandType.Text;
            command.AddParameter("@Id", addition.Id.Value);
            return command.ExecuteScalarAsync();
        }

        public async Task<IEnumerable<Addition>> GetAllAsync()
        {
            var sql = "SELECT Id, AdditionName, Price, AdditionKind FROM additions";
            var command = _dbConnection.CreateCommand();
            command.CommandText = sql;
            command.CommandType = CommandType.Text;
            var reader = await command.ExecuteReaderAsync();

            if (!reader.HasRows)
            {
                return new List<Addition>();
            }

            var list = new List<Addition>();
            while (reader.Read())
            {
                list.Add(new Addition(reader.GetGuid("Id"),
                    reader.GetString("AdditionName"), reader.GetDecimal("Price"),
                    reader.GetString("AdditionKind")));
            }

            return list;
        }

        public async Task<Addition?> GetAsync(Guid id)
        {
            var sql = "SELECT Id, AdditionName, Price, AdditionKind FROM additions WHERE Id = @Id";
            var command = _dbConnection.CreateCommand();
            command.CommandText = sql;
            command.CommandType = CommandType.Text;
            command.AddParameter("@Id", id);
            var reader = await command.ExecuteReaderAsync();
            Addition? addition = null;

            if (!reader.HasRows)
            {
                return addition;
            }

            while (reader.Read())
            {
                addition = new Addition(reader.GetGuid("Id"),
                    reader.GetString("AdditionName"), reader.GetDecimal("Price"),
                    reader.GetString("AdditionKind"));
            }

            return addition;
        }

        public Task UpdateAsync(Addition addition)
        {
            var sql = "UPDATE additions SET AdditionName = @AdditionName, Price = @Price, AdditionKind = @AdditionKind WHERE Id = @Id";
            var command = _dbConnection.CreateCommand();
            command.CommandText = sql;
            command.CommandType = CommandType.Text;
            command.AddParameter("@Id", addition.Id.Value);
            command.AddParameter("@AdditionName", addition.AdditionName.Value);
            command.AddParameter("@Price", addition.Price.Value);
            command.AddParameter("@AdditionKind", addition.AdditionKind.ToString());
            return command.ExecuteScalarAsync();
        }
    }
}
