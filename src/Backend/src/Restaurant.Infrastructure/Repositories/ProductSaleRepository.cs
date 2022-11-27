using Microsoft.Extensions.Logging;
using Restaurant.Core.Entities;
using Restaurant.Core.Repositories;
using Restaurant.Core.ValueObjects;
using System.Data;
using System.Data.Common;
using System.Reflection;

namespace Restaurant.Infrastructure.Repositories
{
    internal class ProductSaleRepository : IProductSaleRepository
    {
        private readonly DbConnection _dbConnection;
        private readonly ILogger<ProductSaleRepository> _logger;

        public ProductSaleRepository(DbConnection dbConnection, ILogger<ProductSaleRepository> logger)
        {
            _dbConnection = dbConnection;
            _logger = logger;
        }

        public Task AddAsync(ProductSale productSale)
        {
            var sql = "INSERT INTO product_sales (Id, ProductId, OrderId, AdditionId, EndPrice, Email, ProductSaleState) VALUES (@Id, @ProductId, @OrderId, @AdditionId, @EndPrice, @Email, @ProductSaleState)";
            var command = _dbConnection.CreateCommand();
            command.CommandText = sql;
            command.CommandType = CommandType.Text;
            command.AddParameter("@Id", productSale.Id.Value);
            command.AddParameter("@ProductId", productSale.ProductId.Value);
            command.AddParameter("@OrderId", productSale.OrderId?.Value);
            command.AddParameter("@AdditionId", productSale.AdditionId?.Value);
            command.AddParameter("@EndPrice", productSale.EndPrice.Value);
            command.AddParameter("@Email", productSale.Email.Value);
            command.AddParameter("@ProductSaleState", productSale.ProductSaleState.ToString());
            _logger.LogInformation($"Infrastructure: Invoking query: {sql}");
            return command.ExecuteScalarAsync();
        }

        public Task DeleteAsync(ProductSale productSale)
        {
            var sql = "DELETE FROM product_sales WHERE Id = @Id";
            var command = _dbConnection.CreateCommand();
            command.CommandText = sql;
            command.CommandType = CommandType.Text;
            command.AddParameter("@Id", productSale.Id.Value);
            _logger.LogInformation($"Infrastructure: Invoking query: {sql}");
            return command.ExecuteScalarAsync();
        }

        public async Task<IEnumerable<ProductSale>> GetAllAsync()
        {
            var sql = "SELECT Id, ProductId, OrderId, AdditionId, EndPrice, Email, ProductSaleState FROM product_sales";
            var command = _dbConnection.CreateCommand();
            command.CommandText = sql;
            command.CommandType = CommandType.Text;
            _logger.LogInformation($"Infrastructure: Invoking query: {sql}");
            using var reader = await command.ExecuteReaderAsync();

            if (!reader.HasRows)
            {
                return new List<ProductSale>();
            }

            var list = new List<ProductSale>();
            while (reader.Read())
            {
                var additionId = reader.GetSafeGuid("AdditionId");
                var orderId = reader.GetSafeGuid("OrderId");
                list.Add(new ProductSale(reader.GetGuid("Id"),
                    reader.GetGuid("ProductId"),
                    Enum.Parse<ProductSaleState>(reader.GetString("ProductSaleState")),
                    reader.GetDecimal("EndPrice"),
                    Email.Of(reader.GetString("Email")),
                    additionId != Guid.Empty ? additionId : null,
                    orderId != Guid.Empty ? orderId : null));
            }

            return list;
        }

        public async Task<IEnumerable<ProductSale>> GetAllByEmailAsync(string email)
        {
            var sql = "SELECT Id, ProductId, OrderId, AdditionId, EndPrice, Email, ProductSaleState FROM product_sales WHERE Email = @Email";
            var command = _dbConnection.CreateCommand();
            command.CommandText = sql;
            command.CommandType = CommandType.Text;
            command.AddParameter("@Email", email);
            _logger.LogInformation($"Infrastructure: Invoking query: {sql}");
            using var reader = await command.ExecuteReaderAsync();

            if (!reader.HasRows)
            {
                return new List<ProductSale>();
            }

            var list = new List<ProductSale>();
            while (reader.Read())
            {
                var additionId = reader.GetSafeGuid("AdditionId");
                var orderId = reader.GetSafeGuid("OrderId");
                list.Add(new ProductSale(reader.GetGuid("Id"),
                    reader.GetGuid("ProductId"),
                    Enum.Parse<ProductSaleState>(reader.GetString("ProductSaleState")),
                    reader.GetDecimal("EndPrice"),
                    Email.Of(reader.GetString("Email")),
                    additionId != Guid.Empty ? additionId : null,
                    orderId != Guid.Empty ? orderId : null));
            }

            return list;
        }

        public async Task<IEnumerable<ProductSale>> GetAllByOrderIdAsync(Guid orderId)
        {
            var sql = "SELECT Id, ProductId, OrderId, AdditionId, EndPrice, Email, ProductSaleState FROM product_sales WHERE OrderId = @OrderId";
            var command = _dbConnection.CreateCommand();
            command.CommandText = sql;
            command.CommandType = CommandType.Text;
            command.AddParameter("@OrderId", orderId);
            _logger.LogInformation($"Infrastructure: Invoking query: {sql}");
            using var reader = await command.ExecuteReaderAsync();

            if (!reader.HasRows)
            {
                return new List<ProductSale>();
            }

            var list = new List<ProductSale>();
            while (reader.Read())
            {
                var additionId = reader.GetSafeGuid("AdditionId");
                list.Add(new ProductSale(reader.GetGuid("Id"),
                    reader.GetGuid("ProductId"),
                    Enum.Parse<ProductSaleState>(reader.GetString("ProductSaleState")),
                    reader.GetDecimal("EndPrice"),
                    Email.Of(reader.GetString("Email")),
                    additionId,
                    orderId));
            }

            return list;
        }

        public async Task<ProductSale?> GetAsync(Guid id)
        {
            var sql = """
                      SELECT p.Id as `p.Id`, p.ProductName as `p.ProductName`, p.Price as `p.Price`, p.ProductKind as `p.ProductKind`, 
                      ps.Id as `ps.Id`, ps.ProductId as `ps.ProductId`, ps.AdditionId as `ps.AdditionId`, ps.EndPrice as `ps.EndPrice`, ps.OrderId as `ps.OrderId`, ps.ProductSaleState as `ps.ProductSaleState`, ps.Email as `ps.Email`,
                      a.Id as `a.Id`, a.AdditionName as `a.AdditionName`, a.Price as `a.Price`, a.AdditionKind as `a.AdditionKind`, 
                      o.Id as `o.Id`, o.OrderNumber as `o.OrderNumber`, o.Created as `o.Created`, o.Price as `o.Price`, o.Email as `o.Email`, o.Note as `o.Note`
                      FROM product_sales ps
                      LEFT JOIN products p ON ps.ProductId = p.Id
                      LEFT JOIN additions a on ps.AdditionId = a.Id
                      LEFT JOIN orders o ON o.Id = ps.OrderId
                      """;
            var command = _dbConnection.CreateCommand();
            command.CommandText = sql;
            command.CommandType = CommandType.Text;
            command.AddParameter("@Id", id);
            _logger.LogInformation($"Infrastructure: Invoking query: {sql}");
            using var reader = await command.ExecuteReaderAsync();

            ProductSale? productSale = null;

            if (!reader.HasRows)
            {
                return productSale;
            }

            while (reader.Read())
            {
                Order? order = null;
                Addition? addition = null;
                Product? product = null;

                product ??= ConstructProductFromQuery(reader, "p");
                var additionId = reader.GetSafeGuid("a.Id");
                if (additionId is not null)
                {
                    addition = ConstructAdditionFromQuery(reader, "a");
                }

                var orderId = reader.GetSafeGuid("o.Id");
                if (orderId is not null)
                {
                    order = ConstructOrderFromQuery(reader, "o");
                }

                productSale = ConstructProductSaleFromQuery(reader, "ps", product, addition, order);
            }

            return productSale;
        }

        public Task UpdateAsync(ProductSale productSale)
        {
            var sql = "UPDATE product_sales SET ProductId = @ProductId, OrderId = @OrderId, AdditionId = @AdditionId, EndPrice = @EndPrice, Email = @Email, ProductSaleState = @ProductSaleState WHERE Id = @Id";
            var command = _dbConnection.CreateCommand();
            command.CommandText = sql;
            command.CommandType = CommandType.Text;
            command.AddParameter("@Id", productSale.Id.Value);
            command.AddParameter("@ProductId", productSale.ProductId.Value);
            command.AddParameter("@OrderId", productSale.OrderId?.Value);
            command.AddParameter("@AdditionId", productSale.AdditionId?.Value);
            command.AddParameter("@EndPrice", productSale.EndPrice.Value);
            command.AddParameter("@Email", productSale.Email.Value);
            command.AddParameter("@ProductSaleState", productSale.ProductSaleState.ToString());
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

        private Product ConstructProductFromQuery(DbDataReader reader, string? prefix = null, IEnumerable<Order>? orders = null, IEnumerable<EntityId>? productSaleIds = null)
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
