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
    internal sealed class AdditionRepository : IAdditonRepository
    {
        private readonly DbConnection _dbConnection;
        private readonly ILogger<AdditionRepository> _logger;

        public AdditionRepository(DbConnection dbConnection, ILogger<AdditionRepository> logger)
        {
            _dbConnection = dbConnection;
            _logger = logger;
        }

        public Task AddAsync(Addition addition)
        {
            var sql = "INSERT INTO additions (Id, AdditionName, Price, AdditionKind) VALUES (@Id, @AdditionName, @Price, @AdditionKind)";
            _logger.LogInformation($"Infrastructure: Invoking query: {sql}");
            return _dbConnection.ExecuteAsync(sql, new { Id = addition.Id.Value, AdditionName = addition.AdditionName.Value, Price = addition.Price.Value, 
                       AdditionKind = addition.AdditionKind.ToString() });
        }

        public Task DeleteAsync(Addition addition)
        {
            var sql = "DELETE FROM additions WHERE Id = @Id";
            _logger.LogInformation($"Infrastructure: Invoking query: {sql}");
            return _dbConnection.ExecuteAsync(sql, new { Id = addition.Id.Value });
        }

        public async Task<IEnumerable<Addition>> GetAllAsync()
        {
            var sql = "SELECT Id, AdditionName, Price, AdditionKind FROM additions";
            _logger.LogInformation($"Infrastructure: Invoking query: {sql}");
            return (await _dbConnection.QueryAsync<AdditionDBO>(sql))
                .Select(a => new Addition(a.Id, a.AdditionName, a.Price, a.AdditionKind));
        }

        public async Task<Addition?> GetAsync(Guid id)
        {
            var sql = """
                        SELECT a.Id, a.AdditionName, a.Price, a.AdditionKind,
                        ps.Id
                        FROM additions a
                        LEFT JOIN product_sales ps on a.Id = ps.AdditionId
                        WHERE a.Id = @Id
                      """;
            _logger.LogInformation($"Infrastructure: Invoking query: {sql}");

            var lookup = new Dictionary<Guid, AdditionDBO>();
            var addition = (await _dbConnection.QueryAsync<AdditionDBO, ProductSaleDBO, AdditionDBO>(sql, (a, ps) =>
            {
                AdditionDBO addition;
                if (!lookup.TryGetValue(a.Id, out addition!))
                {
                    lookup.Add(id, addition = a);
                }

                if (ps is not null)
                {
                    addition.ProductSales.Add(ps);
                }

                return addition;
            }, new { Id = id })).FirstOrDefault();
            return addition is not null ?
                new Addition(addition.Id, addition.AdditionName, addition.Price, addition.AdditionKind, addition.ProductSales?.Select(ps => new EntityId(ps.Id)))
                : null;
        }

        public Task UpdateAsync(Addition addition)
        {
            var sql = "UPDATE additions SET AdditionName = @AdditionName, Price = @Price, AdditionKind = @AdditionKind WHERE Id = @Id";
            _logger.LogInformation($"Infrastructure: Invoking query: {sql}");
            return _dbConnection.ExecuteAsync(sql, new { Id = addition.Id.Value, AdditionName = addition.AdditionName.Value, Price = addition.Price.Value, AdditionKind = addition.AdditionKind.ToString() });
        }
    }
}
