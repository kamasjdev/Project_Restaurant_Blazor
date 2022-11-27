using Microsoft.Extensions.Logging;
using Restaurant.Core.Entities;
using Restaurant.Core.Repositories;
using Restaurant.Core.ValueObjects;
using System.Data;
using System.Data.Common;
using System.Data.SqlServerCe;
using System.Reflection;

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
                        SELECT p.Id as `p.Id`, p.ProductName as `p.ProductName`, p.Price as `p.Price`, p.ProductKind as `p.ProductKind`, 
                        ps.Id as `ps.Id`, ps.ProductId as `ps.ProductId`, ps.AdditionId as `ps.AdditionId`, ps.EndPrice as `ps.EndPrice`, ps.OrderId as `ps.OrderId`, ps.ProductSaleState as `ps.ProductSaleState`, ps.Email as `ps.Email`,
                        a.Id as `a.Id`, a.AdditionName as `a.AdditionName`, a.Price as `a.Price`, a.AdditionKind as `a.AdditionKind`, 
                        o.Id as `o.Id`, o.OrderNumber as `o.OrderNumber`, o.Created as `o.Created`, o.Price as `o.Price`, o.Email as `o.Email`, o.Note as `o.Note`
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
                if (product is null)
                {
                    product = new Product(reader.GetSafeGuid("p.Id"),
                        reader.GetSafeString("p.ProductName"),
                        reader.GetSafeDecimal("p.Price"),
                            reader.GetString("p.ProductKind"));
                    var ordersField = typeof(Product).GetField("_orders", BindingFlags.NonPublic | BindingFlags.Instance);
                    ordersField?.SetValue(product, orders);
                    var productSaleIdsField = typeof(Product).GetField("_productSaleIds", BindingFlags.NonPublic | BindingFlags.Instance);
                    productSaleIdsField?.SetValue(product, productSaleIds);
                }

                var productSaleId = reader.GetSafeGuid("ps.Id");
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
                var additionId = reader.GetSafeGuid("a.Id");

                if (additionId != Guid.Empty)
                {
                    addition = new Addition(additionId, reader.GetSafeString("a.AdditionName"), reader.GetDecimal("a.Price"),
                            reader.GetString("a.AdditionKind"));
                    var productSaleIdsField = typeof(Addition).GetField("_productSaleIds", BindingFlags.NonPublic | BindingFlags.Instance);
                    productSaleIdsField?.SetValue(addition, productSaleIds);
                }
                

                var productSale = new ProductSale(productSaleId,
                    product, Enum.Parse<ProductSaleState>(reader.GetString("ps.ProductSaleState")),
                    Email.Of(reader.GetString("ps.Email")),
                    addition);
                productSales.Add(productSale);
                productSaleIds.Add(productSaleId);

                var orderId = reader.GetSafeGuid("o.Id");
                var orderExists = orders.SingleOrDefault(o => o.Id == orderId);
                if (orderExists is not null)
                {
                    orderExists.AddProduct(productSale);
                    continue;
                }

                if (orderId == Guid.Empty)
                {
                    continue;
                }
                
                order = new Order(orderId,
                                reader.GetSafeString("o.OrderNumber"),
                                reader.GetSafeDateTime("o.Created"),
                                reader.GetSafeDecimal("o.Price"),
                                Email.Of(reader.GetString("o.Email")),
                                reader.GetSafeString("o.Note"),
                                productSales.Where(o => o.Order is null));
                orders.Add(order);
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
