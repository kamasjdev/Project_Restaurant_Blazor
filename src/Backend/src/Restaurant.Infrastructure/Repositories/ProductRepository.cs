using Dapper;
using Microsoft.Extensions.Logging;
using Restaurant.Core.Entities;
using Restaurant.Core.Repositories;
using Restaurant.Infrastructure.Repositories.DBO;
using System.Data;
using System.Data.Common;

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
            _logger.LogInformation($"Infrastructure: Invoking query: {sql}");
            return _dbConnection.ExecuteAsync(sql, new
            {
                Id = product.Id.Value,
                ProductName = product.ProductName.Value,
                Price = product.Price.Value,
                ProductKind = product.ProductKind.ToString()
            });
        }

        public Task DeleteAsync(Product product)
        {
            var sql = "DELETE FROM products WHERE Id = @Id";
            _logger.LogInformation($"Infrastructure: Invoking query: {sql}");
            return _dbConnection.ExecuteAsync(sql, new { Id = product.Id.Value });
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            var sql = "SELECT Id, ProductName, Price, ProductKind FROM products";
            _logger.LogInformation($"Infrastructure: Invoking query: {sql}");
            return (await _dbConnection.QueryAsync<ProductDBO>(sql))
                           .Select(p => new Product(p.Id, p.ProductName, p.Price, p.ProductKind));
        }

        public async Task<Product?> GetAsync(Guid id)
        {
            var sql = """
                        SELECT p.Id, p.ProductName, p.Price, p.ProductKind, 
                        ps.Id, ps.ProductId, ps.AdditionId, ps.EndPrice, ps.OrderId, ps.ProductSaleState, ps.Email,
                        a.Id, a.AdditionName, a.Price, a.AdditionKind, 
                        o.Id, o.OrderNumber, o.Created, o.Price, o.Email, o.Note
                        FROM products p
                        LEFT JOIN product_sales ps ON ps.ProductId = p.Id
                        LEFT JOIN additions a on ps.AdditionId = a.Id
                        LEFT JOIN orders o ON o.Id = ps.OrderId
                        WHERE p.Id = @Id
                        """;
            var lookup = new Dictionary<Guid, ProductDBO>();
            var productData = (await _dbConnection.QueryAsync<ProductDBO, ProductSaleDBO, AdditionDBO, OrderDBO, ProductDBO>(sql, (p, ps, a, o) =>
            {
                ProductDBO product;
                if (!lookup.TryGetValue(p.Id, out product!))
                {
                    lookup.Add(id, product = p);
                }

                if (ps is not null)
                {
                    product.ProductSales.Add(ps);
                }

                if (a is not null)
                {
                    ps!.Addition = a;
                }

                if (o is not null)
                {
                    ps!.Order = o;
                }

                return product;
            }, new { Id = id })).FirstOrDefault();

            if (productData is null)
            {
                return null;
            }

            return productData.AsDetailsEntity();
        }

        public Task UpdateAsync(Product product)
        {
            var sql = "UPDATE products SET ProductName = @ProductName, Price = @Price, ProductKind = @ProductKind WHERE Id = @Id";
            _logger.LogInformation($"Infrastructure: Invoking query: {sql}");
            return _dbConnection.ExecuteAsync(sql, new { Id = product.Id.Value, ProductName = product.ProductName.Value, Price = product.Price.Value, 
                    ProductKind = product.ProductKind.ToString() });
        }
    }
}
