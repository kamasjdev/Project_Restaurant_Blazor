using Microsoft.Extensions.Logging;
using Restaurant.Core.Entities;
using Restaurant.Core.Repositories;
using Restaurant.Core.ValueObjects;
using System.Data;
using System.Data.Common;

namespace Restaurant.Infrastructure.Repositories
{
    internal class OrderRepository : IOrderRepository
    {
        private readonly DbConnection _dbConnection;
        private readonly ILogger<OrderRepository> _logger;

        public OrderRepository(DbConnection dbConnection, ILogger<OrderRepository> logger)
        {
            _dbConnection = dbConnection;
            _logger = logger;
        }

        public Task AddAsync(Order order)
        {
            var sql = "INSERT INTO orders (Id, OrderNumber, Created, Price, Email, Note) VALUES (@Id, @OrderNumber, @Created, @Price, @Email, @Note)";
            var command = _dbConnection.CreateCommand();
            command.CommandText = sql;
            command.CommandType = CommandType.Text;
            command.AddParameter("@Id", order.Id.Value);
            command.AddParameter("@OrderNumber", order.OrderNumber.Value);
            command.AddParameter("@Created", order.Created);
            command.AddParameter("@Price", order.Price.Value);
            command.AddParameter("@Email", order.Email.Value);
            command.AddParameter("@Note", order.Note);
            _logger.LogInformation($"Infrastructure: Invoking query: {sql}");
            return command.ExecuteScalarAsync();
        }

        public Task DeleteAsync(Order order)
        {
            var sql = "DELETE FROM orders WHERE Id = @Id";
            var command = _dbConnection.CreateCommand();
            command.CommandText = sql;
            command.CommandType = CommandType.Text;
            command.AddParameter("@Id", order.Id.Value);
            _logger.LogInformation($"Infrastructure: Invoking query: {sql}");
            return command.ExecuteScalarAsync();
        }

        public async Task<IEnumerable<Order>> GetAllAsync()
        {
            var sql = "SELECT Id, OrderNumber, Created, Price, Email, Note FROM orders";
            var command = _dbConnection.CreateCommand();
            command.CommandText = sql;
            command.CommandType = CommandType.Text;
            _logger.LogInformation($"Infrastructure: Invoking query: {sql}");
            using var reader = await command.ExecuteReaderAsync();

            if (!reader.HasRows)
            {
                return new List<Order>();
            }

            var list = new List<Order>();
            while (reader.Read())
            {
                list.Add(new Order(reader.GetGuid("Id"),
                    reader.GetString("OrderNumber"),
                    reader.GetDateTime("Created"),
                    reader.GetDecimal("Price"),
                    Email.Of(reader.GetString("Email")),
                    reader.GetString("Note")));
            }

            return list;
        }

        public async Task<Order?> GetAsync(Guid id)
        {
            var sql = """
                @"SELECT o.Id, o.OrderNumber, o.Created, o.Price, o.Email, o.Note, 
                ps.Id, ps.ProductId, ps.AdditionId, ps.EndPrice, ps.OrderId, ps.ProductSaleState, ps.Email
                a.Id, a.AdditionName, a.Price, a.AdditionKind, 
                p.Id, p.ProductName, p.Price, p.ProductKind
                FROM orders o
                LEFT JOIN product_sales ps ON ps.OrderId = o.Id
                LEFT JOIN additions a on ps.AdditionId = a.Id
                LEFT JOIN products p ON p.Id = ps.ProductId
                WHERE o.Id = @Id";
                """;
            var command = _dbConnection.CreateCommand();
            command.CommandText = sql;
            command.CommandType = CommandType.Text;
            command.AddParameter("@Id", id);
            _logger.LogInformation($"Infrastructure: Invoking query: {sql}");
            using var reader = await command.ExecuteReaderAsync();

            Order? order = null;
            if (!reader.HasRows)
            {
                return order;
            }

            var productSales = new List<ProductSale>();
            var productSaleIds = new List<EntityId>();
            while (reader.Read())
            {
                order ??= new Order(reader.GetGuid("o.Id"),
                    reader.GetString("o.OrderNumber"),
                    reader.GetDateTime("o.Created"),
                    reader.GetDecimal("o.Price"),
                        Email.Of(reader.GetString("o.Email")),
                        reader.GetString("o.Note"),
                        productSales);

                var productSaleId = reader.GetGuid("ps.Id");
                var productSaleExists = productSales.SingleOrDefault(ps => ps.Id == productSaleId);

                if (productSaleExists is not null)
                {
                    continue;
                }

                if (productSaleId == Guid.Empty)
                {
                    continue;
                }

                Addition? addition = null;
                Product? product = null;
                productSales.Add(new ProductSale(productSaleId,
                    product, Enum.Parse<ProductSaleState>(reader.GetString("ps.ProductSaleState")),
                    Email.Of(reader.GetString("ps.Email")),
                    addition, order));
                productSaleIds.Add(productSaleId);

                product = new Product(reader.GetGuid("p.Id"),
                    reader.GetString("p.ProductName"),
                    reader.GetDecimal("p.Price"),
                        reader.GetString("p.ProductKind"),
                        new List<Order>() { order },
                        productSaleIds);

                var additionId = reader.GetGuid("a.Id");
                addition = additionId != Guid.Empty ?
                    new Addition(additionId, reader.GetString("a.AdditionName"), reader.GetDecimal("a.Price"),
                    reader.GetString("a.AdditionKind"), productSaleIds) : null;
            }

            return order;
        }

        public Task UpdateAsync(Order order)
        {
            var sql = "UPDATE orders SET OrderNumber = @OrderNumber, Created = @Created, Price = @Price, Email = @Email, Note = @Note WHERE Id = @Id";
            var command = _dbConnection.CreateCommand();
            command.CommandText = sql;
            command.CommandType = CommandType.Text;
            command.AddParameter("@Id", order.Id.Value);
            command.AddParameter("@OrderNumber", order.OrderNumber.Value);
            command.AddParameter("@Created", order.Created);
            command.AddParameter("@Price", order.Price.Value);
            command.AddParameter("@Email", order.Email.Value);
            command.AddParameter("@Note", order.Note);
            _logger.LogInformation($"Infrastructure: Invoking query: {sql}");
            return command.ExecuteScalarAsync();
        }
    }
}
