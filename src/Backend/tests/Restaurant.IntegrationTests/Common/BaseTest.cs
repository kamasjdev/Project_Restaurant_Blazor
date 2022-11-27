using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Data.Common;
using System.Data;
using Restaurant.Core.Entities;
using Restaurant.Core.Repositories;
using Restaurant.Core.ValueObjects;

namespace Restaurant.IntegrationTests.Common
{
    [Collection("integration-testing")]
    public abstract class BaseTest : IClassFixture<OptionsProvider>, IDisposable
    {
        protected OptionsProvider OptionsProvider { get; }
        private readonly TestAppFactory _app;
        private IServiceScope? _scope;

        public BaseTest(OptionsProvider optionsProvider)
        {
            OptionsProvider = optionsProvider;
            _app = new TestAppFactory(ConfigureServices);
        }

        protected virtual void ConfigureServices(IServiceCollection services)
        {
            // Configure services if needed
        }

        protected T GetRequiredService<T>() where T : notnull
        {
            _scope ??= _app.Services.CreateScope();
            return _scope.ServiceProvider.GetRequiredService<T>();
        }


        public void Dispose()
        {
            _scope ??= _app.Services.CreateScope();
            var connection = _scope.ServiceProvider.GetRequiredService<DbConnection>();
            var logger = _scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

            try
            {
                logger.LogInformation($"Dropping test database {connection.Database}");
                var command = connection.CreateCommand();
                command.CommandText = $"DROP DATABASE IF EXISTS `{connection.Database}`";
                command.CommandType = CommandType.Text;
                command.ExecuteNonQuery();
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "There was an error while drop database");
            }
            finally
            {
                logger.LogInformation("Closing connection. Disposing TestAppFactory");
                _scope.Dispose();
                connection.Close();
                connection.Dispose();
                _app.Dispose();
                GC.SuppressFinalize(this);
            }
        }

        public async Task<Addition> AddDefaultAdditionAsync()
        {
            _scope ??= _app.Services.CreateScope();
            var count = Enum.GetNames<AdditionKind>().Length - 1;
            var random = new Random();
            var additonRepository = _scope.ServiceProvider.GetRequiredService<IAdditonRepository>();
            var addition = new Addition(Guid.NewGuid(), $"Addition-{Guid.NewGuid()}", 100M, (AdditionKind)random.Next(0, count));
            await additonRepository.AddAsync(addition);
            return addition;
        }

        public async Task<Product> AddDefaultProductAsync()
        {
            _scope ??= _app.Services.CreateScope();
            var count = Enum.GetNames<ProductKind>().Length - 1;
            var random = new Random();
            var productRepository = _scope.ServiceProvider.GetRequiredService<IProductRepository>();
            var product = new Product(Guid.NewGuid(), $"Product-{Guid.NewGuid()}", 100M, (ProductKind)random.Next(0, count));
            await productRepository.AddAsync(product);
            return product;
        }

        public async Task<ProductSale> AddDefaultProductSaleAsync(EntityId productId, decimal endPrice, Email email, EntityId? additionId = null)
        {
            _scope ??= _app.Services.CreateScope();
            var count = Enum.GetNames<ProductKind>().Length - 1;
            var random = new Random();
            var productSaleRepository = _scope.ServiceProvider.GetRequiredService<IProductSaleRepository>();
            var productSale = new ProductSale(Guid.NewGuid(), productId, (ProductSaleState)random.Next(0, count), endPrice, email, additionId);
            await productSaleRepository.AddAsync(productSale);
            return productSale;
        }

        public async Task<Order> AddDefaultOrderAsync(Email email, IEnumerable<ProductSale> productSales)
        {
            _scope ??= _app.Services.CreateScope();
            var orderRepository = _scope.ServiceProvider.GetRequiredService<IOrderRepository>();
            var productSaleRepository = _scope.ServiceProvider.GetRequiredService<IProductSaleRepository>();
            var order = new Order(Guid.NewGuid(), $"Order-{Guid.NewGuid().ToString("N")}", DateTime.UtcNow, productSales.Sum(p => p.EndPrice), email);
            order.AddProducts(productSales);
            await orderRepository.AddAsync(order);

            var tasks = new List<Task>();
            foreach (var productSale in productSales)
            {
                tasks.Add(productSaleRepository.UpdateAsync(productSale));
            }

            await Task.WhenAll(tasks);
            return order;
        }
    }
}
