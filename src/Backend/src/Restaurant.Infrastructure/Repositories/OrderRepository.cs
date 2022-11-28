using Dapper;
using Microsoft.Extensions.Logging;
using Restaurant.Core.Entities;
using Restaurant.Core.Repositories;
using Restaurant.Core.ValueObjects;
using Restaurant.Infrastructure.Repositories.DBO;
using System.Data;
using System.Data.Common;
using System.Data.SqlServerCe;
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
            _logger.LogInformation($"Infrastructure: Invoking query: {sql}");
            return _dbConnection.ExecuteAsync(sql, new
            {
                Id = order.Id.Value,
                OrderNumber = order.OrderNumber.Value,
                order.Created,
                Price = order.Price.Value,
                Email = order.Email.Value,
                order.Note
            });
        }

        public Task DeleteAsync(Order order)
        {
            var sql = "DELETE FROM orders WHERE Id = @Id";
            _logger.LogInformation($"Infrastructure: Invoking query: {sql}");
            return _dbConnection.ExecuteAsync(sql, new { Id = order.Id.Value });
        }

        public async Task<IEnumerable<Order>> GetAllAsync()
        {
            var sql = "SELECT Id, OrderNumber, Created, Price, Email, Note FROM orders";
            _logger.LogInformation($"Infrastructure: Invoking query: {sql}");
            return (await _dbConnection.QueryAsync<OrderDBO>(sql))
                           .Select(o => new Order(o.Id, o.OrderNumber, o.Created, o.Price, Email.Of(o.Email), o.Note));
        }

        public async Task<Order?> GetAsync(Guid id)
        {
            var sql = """
                SELECT o.Id, o.OrderNumber, o.Created, o.Price, o.Email, o.Note, 
                ps.Id, ps.ProductId, ps.AdditionId, ps.EndPrice, ps.OrderId, ps.ProductSaleState, ps.Email,
                a.Id, a.AdditionName, a.Price, a.AdditionKind, 
                p.Id, p.ProductName, p.Price, p.ProductKind
                FROM orders o
                LEFT JOIN product_sales ps ON ps.OrderId = o.Id
                LEFT JOIN additions a on ps.AdditionId = a.Id
                LEFT JOIN products p ON p.Id = ps.ProductId
                WHERE o.Id = @Id
                """;
            var lookup = new Dictionary<Guid, OrderDBO>();
            var orderData = (await _dbConnection.QueryAsync<OrderDBO, ProductSaleDBO, AdditionDBO, ProductDBO, OrderDBO>(sql, (o, ps, a, p) =>
            {
                OrderDBO order;
                if (!lookup.TryGetValue(o.Id, out order!))
                {
                    lookup.Add(id, order = o);
                }

                if (ps is not null)
                {
                    order.ProductSales.Add(ps);
                    ps.Order = o;
                }

                if (a is not null)
                {
                    ps!.Addition = a;
                }

                if (p is not null)
                {
                    ps!.Product = p;
                }

                return order;
            }, new { Id = id })).FirstOrDefault();

            if (orderData is null)
            {
                return null;
            }

            var productSales = new List<ProductSale>();
            var productSaleIds = new List<EntityId>();
            var productSalesProperty = typeof(Product).GetField("_productSaleIds", BindingFlags.NonPublic | BindingFlags.Instance);
            var ordersProperty = typeof(Product).GetField("_orders", BindingFlags.NonPublic | BindingFlags.Instance);
            var additionProductSalesProperty = typeof(Addition).GetField("_productSaleIds", BindingFlags.NonPublic | BindingFlags.Instance);
            var order = new Order(orderData.Id, orderData.OrderNumber, orderData.Created, orderData.Price, Email.Of(orderData.Email), orderData.Note, productSales);
            var productsProperty = typeof(Order).GetField("_products", BindingFlags.NonPublic | BindingFlags.Instance);
            productsProperty?.SetValue(order, productSales);

            foreach (var productSale in orderData.ProductSales)
            {
                productSaleIds.Add(productSale.Id);
                var product = new Product(productSale.Product!.Id, productSale.Product.ProductName, productSale.Product.Price, productSale.Product.ProductKind, new List<Order> { order });
                productSalesProperty?.SetValue(product, productSaleIds);

                Addition? addition = null;
                if (productSale.Addition is not null)
                {
                    addition = new Addition(productSale.Addition.Id, productSale.Addition.AdditionName, productSale.Addition.Price, productSale.Addition.AdditionKind);
                    additionProductSalesProperty?.SetValue(addition, productSaleIds);
                }

                productSales.Add(new ProductSale(productSale.Id, product, productSale.ProductSaleState, Email.Of(productSale.Email), addition, order));
            }
            return order;
        }

        public Task UpdateAsync(Order order)
        {
            var sql = "UPDATE orders SET OrderNumber = @OrderNumber, Created = @Created, Price = @Price, Email = @Email, Note = @Note WHERE Id = @Id";
            _logger.LogInformation($"Infrastructure: Invoking query: {sql}");
            return _dbConnection.ExecuteAsync(sql, new
            {
                Id = order.Id.Value,
                OrderNumber = order.OrderNumber.Value,
                order.Created,
                Price = order.Price.Value,
                Email = order.Email.Value,
                order.Note
            });
        }
    }
}
