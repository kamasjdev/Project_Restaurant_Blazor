using Microsoft.Extensions.Logging;
using Restaurant.Core.Entities;
using Restaurant.Core.Repositories;
using Restaurant.Core.ValueObjects;
using System.Data;
using System.Data.Common;

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

                product ??= new Product(reader.GetGuid("p.Id"),
                                    reader.GetString("p.ProductName"),
                                    reader.GetDecimal("p.Price"),
                                        reader.GetString("p.ProductKind"));

                var additionId = reader.GetSafeGuid("a.Id");
                if (additionId is not null)
                {
                    addition = new Addition(additionId, reader.GetString("a.AdditionName"), reader.GetDecimal("a.Price"),
                    reader.GetString("a.AdditionKind"));
                }

                var orderId = reader.GetSafeGuid("o.Id");
                if (orderId is not null)
                {
                    order = new Order(orderId,
                                    reader.GetString("o.OrderNumber"),
                                    reader.GetDateTime("o.Created"),
                                    reader.GetDecimal("o.Price"),
                                    Email.Of(reader.GetString("o.Email")),
                                    reader.GetSafeString("o.Note"));
                }

                productSale = new ProductSale(reader.GetGuid("ps.Id"),
                    product, Enum.Parse<ProductSaleState>(reader.GetString("ps.ProductSaleState")),
                Email.Of(reader.GetString("ps.Email")),
                    addition, order);
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
    }
}
