using Dapper;
using Microsoft.Extensions.Logging;
using Restaurant.Core.Entities;
using Restaurant.Core.Repositories;
using Restaurant.Core.ValueObjects;
using Restaurant.Infrastructure.Repositories.DBO;
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

            var orders = new List<Order>();
            var productSaleIds = new List<EntityId>();
            var product = new Product(productData.Id, productData.ProductName, productData.Price, productData.ProductKind);
            var productSalesProperty = typeof(Product).GetField("_productSaleIds", BindingFlags.NonPublic | BindingFlags.Instance);
            productSalesProperty?.SetValue(product, productSaleIds);
            var ordersProperty = typeof(Product).GetField("_orders", BindingFlags.NonPublic | BindingFlags.Instance);
            ordersProperty?.SetValue(product, orders);
            var additionProductSalesProperty = typeof(Addition).GetField("_productSaleIds", BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var productSaleData in productData.ProductSales)
            {
                if (productSaleData.Order is null)
                {
                    continue;
                }

                productSaleIds.Add(productSaleData.Id);
                Addition? addition = null;
                if (productSaleData.Addition is not null)
                {
                    addition = new Addition(productSaleData.Addition.Id, productSaleData.Addition.AdditionName, productSaleData.Addition.Price, productSaleData.Addition.AdditionKind);
                    additionProductSalesProperty?.SetValue(addition, productSaleIds);
                }

                var orderExists = orders.SingleOrDefault(o => o.Id == productSaleData.OrderId);

                var productSale = new ProductSale(productSaleData.Id, product, productSaleData.ProductSaleState, Email.Of(productSaleData.Email), addition);
                if (orderExists is not null)
                {
                    orderExists.AddProduct(productSale);
                    continue;
                }

                var order = new Order(productSaleData.Order.Id, productSaleData.Order.OrderNumber, productSaleData.Order.Created, productSaleData.Order.Price, Email.Of(productSaleData.Order.Email), productSaleData.Order.Note);
                order.AddProduct(productSale);
                orders.Add(order);
            }

            return product;
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
