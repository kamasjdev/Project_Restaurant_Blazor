using Microsoft.Extensions.Logging;
using Restaurant.Core.Entities;
using Restaurant.Core.Repositories;
using Restaurant.Core.ValueObjects;
using System.Data;
using System.Data.Common;
using System.Data.SqlServerCe;

namespace Restaurant.Infrastructure.Repositories
{
    internal sealed class ProductRepository : IProductRepository
    {
        private readonly DbConnection _dbConnection;
        private readonly ILogger<ProductRepository> _logger;

        public ProductRepository(DbConnection dbConnection, ILogger<ProductRepository> logger)
        {
            _dbConnection = dbConnection;
            _logger = logger;
        }

        public Task AddAsync(Product product)
        {
            var sql = "INSERT INTO products (Id, ProductName, Price, ProductKind) VALUES (@Id, @ProductName, @Price, @ProductKind)";
            var command = _dbConnection.CreateCommand();
            command.CommandText = sql;
            command.CommandType = CommandType.Text;
            command.AddParameter("@Id", product.Id.Value);
            command.AddParameter("@ProductName", product.ProductName.Value);
            command.AddParameter("@Price", product.Price.Value);
            command.AddParameter("@ProductKind", product.ProductKind.ToString());
            _logger.LogInformation($"Infrastructure: Invoking query: {sql}");
            return command.ExecuteScalarAsync();
        }

        public Task DeleteAsync(Product product)
        {
            var sql = "DELETE FROM products WHERE Id = @Id";
            var command = _dbConnection.CreateCommand();
            command.CommandText = sql;
            command.CommandType = CommandType.Text;
            command.AddParameter("@Id", product.Id.Value);
            _logger.LogInformation($"Infrastructure: Invoking query: {sql}");
            return command.ExecuteScalarAsync();
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            var sql = "SELECT Id, ProductName, Price, ProductKind FROM products";
            var command = _dbConnection.CreateCommand();
            command.CommandText = sql;
            command.CommandType = CommandType.Text;
            _logger.LogInformation($"Infrastructure: Invoking query: {sql}");
            using var reader = await command.ExecuteReaderAsync();

            if (!reader.HasRows)
            {
                return new List<Product>();
            }

            var list = new List<Product>();
            while (reader.Read())
            {
                list.Add(new Product(reader.GetGuid("Id"),
                    reader.GetString("ProductName"), 
                    reader.GetDecimal("Price"),
                    reader.GetString("ProductKind")));
            }

            return list;
        }

        public async Task<Product?> GetAsync(Guid id)
        {
            var sql = """
                        SELECT p.Id, p.ProductName, p.Price, p.ProductKind, 
                        ps.Id, ps.ProductId, ps.AdditionId, ps.EndPrice, ps.OrderId, ps.ProductSaleState, ps.Email
                        a.Id, a.AdditionName, a.Price, a.AdditionKind, 
                        o.Id, o.OrderNumber, o.Created, o.Price, o.Email, o.Note
                        FROM products p
                        LEFT JOIN product_sales ps ON ps.ProductId = p.Id
                        LEFT JOIN additions a on ps.AdditionId = a.Id
                        LEFT JOIN orders o ON o.Id = ps.OrderId
                        WHERE p.Id = @Id
                        """;
            var command = _dbConnection.CreateCommand();
            command.CommandText = sql;
            command.CommandType = CommandType.Text;
            command.AddParameter("@Id", id);
            _logger.LogInformation($"Infrastructure: Invoking query: {sql}");
            using var reader = await command.ExecuteReaderAsync();

            Product? product = null;
            if (!reader.HasRows)
            {
                return product;
            }

            var orders = new List<Order>();
            var productSales = new List<ProductSale>();
            var productSaleIds = new List<EntityId>();
            while (reader.Read())
            {
                product ??= new Product(reader.GetGuid("p.Id"),
                    reader.GetString("p.ProductName"),
                    reader.GetDecimal("p.Price"),
                        reader.GetString("p.ProductKind"), 
                        orders,
                        productSaleIds);

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

                Order? order = null;
                Addition? addition = null;
                productSales.Add(new ProductSale(productSaleId,
                    product, Enum.Parse<ProductSaleState>(reader.GetString("ps.ProductSaleState")),
                    Email.Of(reader.GetString("ps.Email")),
                    addition, order));
                productSaleIds.Add(productSaleId);

                var additionId = reader.GetGuid("a.Id");
                addition = additionId != Guid.Empty ?
                    new Addition(additionId, reader.GetString("a.AdditionName"), reader.GetDecimal("a.Price"),
                    reader.GetString("a.AdditionKind"), productSaleIds) : null;


                var orderId = reader.GetGuid("o.Id");
                var orderExists = orders.SingleOrDefault(o => o.Id == orderId);
                if (orderExists is not null)
                {
                    continue;
                }

                if (orderId == Guid.Empty)
                {
                    continue;
                }
                
                order = new Order(reader.GetGuid("o.Id"),
                                reader.GetString("o.OrderNumber"),
                                reader.GetDateTime("o.Created"),
                                reader.GetDecimal("o.Price"),
                                Email.Of(reader.GetString("o.Email")),
                                reader.GetString("o.Note"),
                                productSales);
            }

            return product;
        }

        public Task UpdateAsync(Product product)
        {
            var sql = "UPDATE products SET ProductName = @ProductName, Price = @Price, ProductKind = @ProductKind WHERE Id = @Id";
            var command = _dbConnection.CreateCommand();
            command.CommandText = sql;
            command.CommandType = CommandType.Text;
            command.AddParameter("@Id", product.Id.Value);
            command.AddParameter("@ProductName", product.ProductName.Value);
            command.AddParameter("@Price", product.Price.Value);
            command.AddParameter("@ProductKind", product.ProductKind.ToString());
            _logger.LogInformation($"Infrastructure: Invoking query: {sql}");
            return command.ExecuteScalarAsync();
        }
    }
}
