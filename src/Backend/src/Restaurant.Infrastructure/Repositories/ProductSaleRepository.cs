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
            _logger.LogInformation($"Infrastructure: Invoking query: {sql}");
            return _dbConnection.ExecuteAsync(sql, new { Id = productSale.Id.Value, ProductId = productSale.ProductId.Value, OrderId = productSale.OrderId?.Value,
                AdditionId = productSale.AdditionId?.Value, EndPrice = productSale.EndPrice.Value, Email = productSale.Email.Value, 
                ProductSaleState = productSale.ProductSaleState.ToString() 
            });
        }

        public Task DeleteAsync(ProductSale productSale)
        {
            var sql = "DELETE FROM product_sales WHERE Id = @Id";
            _logger.LogInformation($"Infrastructure: Invoking query: {sql}");
            return _dbConnection.ExecuteAsync(sql, new { Id = productSale.Id.Value });
        }

        public Task DeleteByOrderAsync(Guid orderId)
        {
            var sql = "DELETE FROM product_sales WHERE OrderId = @OrderId";
            _logger.LogInformation($"Infrastructure: Invoking query: {sql}");
            return _dbConnection.ExecuteAsync(sql, new { OrderId = orderId });
        }

        public async Task<IEnumerable<ProductSale>> GetAllAsync()
        {
            var sql = "SELECT Id, ProductId, OrderId, AdditionId, EndPrice, Email, ProductSaleState FROM product_sales";
            _logger.LogInformation($"Infrastructure: Invoking query: {sql}");
            return (await _dbConnection.QueryAsync<ProductSaleDBO>(sql))
                           .Select(ps => new ProductSale(ps.Id, ps.ProductId, ps.ProductSaleState, ps.EndPrice, Email.Of(ps.Email), ps.AdditionId, ps.OrderId));
        }

        public async Task<IEnumerable<ProductSale>> GetAllInCartByEmailAsync(string email)
        {
            var sql = """
                SELECT ps.Id, ps.ProductId, ps.OrderId, ps.AdditionId, ps.EndPrice, ps.Email, ProductSaleState,
                p.Id, p.ProductKind, p.ProductName, p.Price,
                a.Id, a.AdditionKind, a.AdditionName, a.Price
                FROM product_sales ps
                JOIN products p on p.Id = ps.ProductId
                LEFT JOIN additions a on a.Id = ps.AdditionId
                WHERE Email = @Email AND OrderId IS NULL
                """;
            _logger.LogInformation($"Infrastructure: Invoking query: {sql}");
            var lookup = new Dictionary<Guid, ProductSaleDBO>();
            var productSaleData = (await _dbConnection.QueryAsync<ProductSaleDBO, ProductDBO, AdditionDBO, ProductSaleDBO>(sql, (ps, p, a) =>
            {
                ProductSaleDBO productSale;
                if (!lookup.TryGetValue(ps.Id, out productSale!))
                {
                    lookup.Add(ps.Id, productSale = ps);
                }

                if (p is not null)
                {
                    productSale.Product = p;
                }

                if (a is not null)
                {
                    productSale.Addition = a;
                }

                return productSale;
            }, new { Email = email })).ToList();

            return productSaleData.Select(ps => ps.AsDetailsEntity());
        }

        public async Task<IEnumerable<ProductSale>> GetAllByOrderIdAsync(Guid orderId)
        {
            var sql = "SELECT Id, ProductId, OrderId, AdditionId, EndPrice, Email, ProductSaleState FROM product_sales WHERE OrderId = @OrderId";
            _logger.LogInformation($"Infrastructure: Invoking query: {sql}");
            return (await _dbConnection.QueryAsync<ProductSaleDBO>(sql, new { OrderId = orderId }))
                           .Select(ps => new ProductSale(ps.Id, ps.ProductId, ps.ProductSaleState, ps.EndPrice, Email.Of(ps.Email), ps.AdditionId, ps.OrderId));
        }

        public async Task<ProductSale?> GetAsync(Guid id)
        {
            var sql = """
                      SELECT ps.Id, ps.ProductId, ps.AdditionId, ps.EndPrice, ps.OrderId, ps.ProductSaleState, ps.Email,
                      p.Id, p.ProductName, p.Price, p.ProductKind,
                      a.Id, a.AdditionName, a.Price, a.AdditionKind, 
                      o.Id, o.OrderNumber, o.Created, o.Price, o.Email, o.Note
                      FROM product_sales ps
                      JOIN products p ON ps.ProductId = p.Id
                      LEFT JOIN additions a on ps.AdditionId = a.Id
                      LEFT JOIN orders o ON o.Id = ps.OrderId
                      WHERE ps.Id = @Id
                      """;
            var lookup = new Dictionary<Guid, ProductSaleDBO>();
            var productSaleData = (await _dbConnection.QueryAsync<ProductSaleDBO, ProductDBO, AdditionDBO, OrderDBO, ProductSaleDBO>(sql, (ps, p, a, o) =>
            {
                ProductSaleDBO productSale;
                if (!lookup.TryGetValue(ps.Id, out productSale!))
                {
                    lookup.Add(id, productSale = ps);
                }

                if (p is not null)
                {
                    productSale.Product = p;
                }

                if (a is not null)
                {
                    productSale.Addition = a;
                }

                if (o is not null)
                {
                    productSale.Order = o;
                }

                return productSale;
            }, new { Id = id })).FirstOrDefault();

            if (productSaleData is null)
            {
                return null;
            }

            return productSaleData.AsDetailsEntity();
        }

        public Task UpdateAsync(ProductSale productSale)
        {
            var sql = "UPDATE product_sales SET ProductId = @ProductId, OrderId = @OrderId, AdditionId = @AdditionId, EndPrice = @EndPrice, Email = @Email, ProductSaleState = @ProductSaleState WHERE Id = @Id";
            _logger.LogInformation($"Infrastructure: Invoking query: {sql}");
            return _dbConnection.ExecuteAsync(sql, new { Id = productSale.Id.Value, ProductId = productSale.ProductId.Value,
                OrderId = productSale.OrderId?.Value, AdditionId  = productSale.AdditionId?.Value, EndPrice = productSale.EndPrice.Value,
                Email = productSale.Email.Value, ProductSaleState = productSale.ProductSaleState.ToString() });
        }
    }
}
