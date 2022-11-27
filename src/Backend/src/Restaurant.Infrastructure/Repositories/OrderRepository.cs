using Microsoft.Extensions.Logging;
using Restaurant.Core.Entities;
using Restaurant.Core.Repositories;
using Restaurant.Core.ValueObjects;
using System.Data;
using System.Data.Common;
using System.Reflection;

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
                list.Add(ConstructOrderFromQuery(reader));
            }

            return list;
        }

        public async Task<Order?> GetAsync(Guid id)
        {
            var sql = """
                SELECT o.Id as `o.Id`, o.OrderNumber as `o.OrderNumber`, o.Created as `o.Created`, o.Price as `o.Price`, o.Email as `o.Email`, o.Note as `o.Note`, 
                ps.Id as `ps.Id`, ps.ProductId as `ps.ProductId`, ps.AdditionId as `ps.AdditionId`, ps.EndPrice as `ps.EndPrice`, ps.OrderId as `ps.OrderId`, ps.ProductSaleState as `ps.ProductSaleState`, ps.Email as `ps.Email`,
                a.Id as `a.Id`, a.AdditionName as `a.AdditionName`, a.Price as `a.Price`, a.AdditionKind as `a.AdditionKind`, 
                p.Id as `p.Id`, p.ProductName as `p.ProductName`, p.Price as `p.Price`, p.ProductKind as `p.ProductKind`
                FROM orders o
                LEFT JOIN product_sales ps ON ps.OrderId = o.Id
                LEFT JOIN additions a on ps.AdditionId = a.Id
                LEFT JOIN products p ON p.Id = ps.ProductId
                WHERE o.Id = @Id
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
                if (order is null)
                {
                    order = ConstructOrderFromQuery(reader, "o", productSales);
                }

                var productSaleId = reader.GetSafeGuid("ps.Id");
                var productSaleExists = productSales.SingleOrDefault(ps => ps.Id == productSaleId);

                if (productSaleExists is not null)
                {
                    continue;
                }

                if (productSaleId is null)
                {
                    continue;
                }

                Addition? addition = null;
                Product? product = null;

                product = ConstructProductFromQuery(reader, "p", order, productSaleIds);
                var additionId = reader.GetSafeGuid("a.Id");

                if (additionId is not null)
                {
                    addition = ConstructAdditionFromQuery(reader, "a", productSaleIds);
                }

                productSales.Add(ConstructProductSaleFromQuery(reader, "ps", product, addition, order));
                productSaleIds.Add(productSaleId);
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

        private Order ConstructOrderFromQuery(DbDataReader reader, string? prefix = null, IEnumerable<ProductSale>? productSales = null)
        {
            var searchPrefix = string.IsNullOrEmpty(prefix) ? string.Empty : $"{prefix}.";
            var order = new Order(reader.GetGuid($"{searchPrefix}Id"),
                        reader.GetString($"{searchPrefix}OrderNumber"),
                        reader.GetDateTime($"{searchPrefix}Created"),
                        reader.GetDecimal($"{searchPrefix}Price"),
                            Email.Of(reader.GetString($"{searchPrefix}Email")),
                            reader.GetSafeString($"{searchPrefix}Note"));
            
            if (productSales is not null)
            {
                var productsField = typeof(Order).GetField("_products", BindingFlags.NonPublic | BindingFlags.Instance);
                productsField?.SetValue(order, productSales);
            }

            return order;
        }

        private Product ConstructProductFromQuery(DbDataReader reader, string? prefix = null, Order? order = null, IEnumerable<EntityId>? productSaleIds = null)
        {
            var searchPrefix = string.IsNullOrEmpty(prefix) ? string.Empty : $"{prefix}.";
            var product = new Product(reader.GetGuid($"{searchPrefix}Id"),
                    reader.GetString($"{searchPrefix}ProductName"),
                    reader.GetDecimal($"{searchPrefix}Price"),
                        reader.GetString($"{searchPrefix}ProductKind"));
            
            if (order is not null)
            {
                var ordersField = typeof(Product).GetField("_orders", BindingFlags.NonPublic | BindingFlags.Instance);
                ordersField?.SetValue(product, new List<Order> { order });
            }

            if (productSaleIds is not null)
            {
                var productSaleIdsField = typeof(Product).GetField("_productSaleIds", BindingFlags.NonPublic | BindingFlags.Instance);
                productSaleIdsField?.SetValue(product, productSaleIds);
            }

            return product;
        }

        private Addition ConstructAdditionFromQuery(DbDataReader reader, string prefix, List<EntityId> productSaleIds)
        {
            var searchPrefix = string.IsNullOrEmpty(prefix) ? string.Empty : $"{prefix}.";
            var addition = new Addition(reader.GetGuid($"{searchPrefix}Id"), reader.GetString($"{searchPrefix}AdditionName"), reader.GetDecimal($"{searchPrefix}Price"),
                        reader.GetString($"{searchPrefix}AdditionKind"));
            var additionProductSaleIdsField = typeof(Addition).GetField("_productSaleIds", BindingFlags.NonPublic | BindingFlags.Instance);
            additionProductSaleIdsField?.SetValue(addition, productSaleIds);
            return addition;
        }

        private ProductSale ConstructProductSaleFromQuery(DbDataReader reader, string prefix, Product product, Addition? addition = null, Order? order = null)
        {
            var searchPrefix = string.IsNullOrEmpty(prefix) ? string.Empty : $"{prefix}.";
            return new ProductSale(reader.GetGuid("ps.Id"),
                    product, Enum.Parse<ProductSaleState>(reader.GetString($"{searchPrefix}ProductSaleState")),
                    Email.Of(reader.GetString($"{searchPrefix}Email")),
                    addition, order);
        }
    }
}
