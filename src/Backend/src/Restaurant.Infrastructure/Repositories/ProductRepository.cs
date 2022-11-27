using Microsoft.Extensions.Logging;
using Restaurant.Core.Entities;
using Restaurant.Core.Repositories;
using Restaurant.Core.ValueObjects;
using System.Data;
using System.Data.Common;
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
                list.Add(ConstructProductFromQuery(reader));
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
                    product = ConstructProductFromQuery(reader, "p", orders, productSaleIds);
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

                Order? order = null;
                Addition? addition = null;
                var additionId = reader.GetSafeGuid("a.Id");

                if (additionId is not null)
                {
                    addition = ConstructAdditionFromQuery(reader, "a", productSaleIds);
                }

                var productSale = ConstructProductSaleFromQuery(reader, "ps", product, addition, order);
                productSales.Add(productSale);
                productSaleIds.Add(productSaleId);

                var orderId = reader.GetSafeGuid("o.Id");
                var orderExists = orders.SingleOrDefault(o => o.Id == orderId);
                if (orderExists is not null)
                {
                    orderExists.AddProduct(productSale);
                    continue;
                }

                if (orderId is null)
                {
                    continue;
                }

                order = ConstructOrderFromQuery(reader, "o", productSales.Where(o => o.Order is null).ToList());
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

        private Order ConstructOrderFromQuery(DbDataReader reader, string? prefix = null, List<ProductSale>? productSales = null)
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

        private Product ConstructProductFromQuery(DbDataReader reader, string? prefix = null, List<Order>? orders = null, List<EntityId>? productSaleIds = null)
        {
            var searchPrefix = string.IsNullOrEmpty(prefix) ? string.Empty : $"{prefix}.";
            var product = new Product(reader.GetGuid($"{searchPrefix}Id"),
                    reader.GetString($"{searchPrefix}ProductName"),
                    reader.GetDecimal($"{searchPrefix}Price"),
                        reader.GetString($"{searchPrefix}ProductKind"));

            if (orders is not null)
            {
                var ordersField = typeof(Product).GetField("_orders", BindingFlags.NonPublic | BindingFlags.Instance);
                ordersField?.SetValue(product, orders);
            }

            if (productSaleIds is not null)
            {
                var productSaleIdsField = typeof(Product).GetField("_productSaleIds", BindingFlags.NonPublic | BindingFlags.Instance);
                productSaleIdsField?.SetValue(product, productSaleIds);
            }

            return product;
        }

        private Addition ConstructAdditionFromQuery(DbDataReader reader, string prefix, List<EntityId>? productSaleIds = null)
        {
            var searchPrefix = string.IsNullOrEmpty(prefix) ? string.Empty : $"{prefix}.";
            var addition = new Addition(reader.GetGuid($"{searchPrefix}Id"), reader.GetString($"{searchPrefix}AdditionName"), reader.GetDecimal($"{searchPrefix}Price"),
                        reader.GetString($"{searchPrefix}AdditionKind"));

            if (productSaleIds is not null)
            {
                var additionProductSaleIdsField = typeof(Addition).GetField("_productSaleIds", BindingFlags.NonPublic | BindingFlags.Instance);
                additionProductSaleIdsField?.SetValue(addition, productSaleIds);
            }
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
