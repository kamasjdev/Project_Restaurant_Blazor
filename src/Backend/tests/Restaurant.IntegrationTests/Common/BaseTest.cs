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
        protected readonly TestAppFactory Fixture;
        protected IServiceScope? Scope;

        public BaseTest(OptionsProvider optionsProvider)
        {
            OptionsProvider = optionsProvider;
            Fixture  = new TestAppFactory(ConfigureServices);
        }

        protected virtual void ConfigureServices(IServiceCollection services)
        {
            // Configure services if needed
        }

        protected T? GetService<T>()
        {
            Scope ??= Fixture.Services.CreateScope();
            return Scope.ServiceProvider.GetService<T>();
        }

        protected T GetRequiredService<T>() where T : notnull
        {
            Scope ??= Fixture.Services.CreateScope();
            return Scope.ServiceProvider.GetRequiredService<T>();
        }


        public virtual void Dispose()
        {
            Scope ??= Fixture.Services.CreateScope();
            var connection = Scope.ServiceProvider.GetRequiredService<DbConnection>();
            var logger = Scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

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
                Scope.Dispose();
                connection.Close();
                connection.Dispose();
                Fixture .Dispose();
                GC.SuppressFinalize(this);
            }
        }

        public async Task<Addition> AddDefaultAdditionAsync()
        {
            Scope ??= Fixture.Services.CreateScope();
            var count = Enum.GetNames<AdditionKind>().Length - 1;
            var random = new Random();
            var additonRepository = Scope.ServiceProvider.GetRequiredService<IAdditonRepository>();
            var addition = new Addition(Guid.NewGuid(), $"Addition-{Guid.NewGuid()}", 100M, (AdditionKind)random.Next(0, count));
            await additonRepository.AddAsync(addition);
            return addition;
        }

        public async Task<Product> AddDefaultProductAsync()
        {
            Scope ??= Fixture.Services.CreateScope();
            var count = Enum.GetNames<ProductKind>().Length - 1;
            var random = new Random();
            var productRepository = Scope.ServiceProvider.GetRequiredService<IProductRepository>();
            var product = new Product(Guid.NewGuid(), $"Product-{Guid.NewGuid()}", 100M, (ProductKind)random.Next(0, count));
            await productRepository.AddAsync(product);
            return product;
        }

        public async Task<ProductSale> AddDefaultProductSaleAsync(EntityId productId, decimal endPrice, Email email, EntityId? additionId = null, EntityId? orderId = null)
        {
            Scope ??= Fixture.Services.CreateScope();
            var count = Enum.GetNames<ProductKind>().Length - 1;
            var random = new Random();
            var productSaleRepository = Scope.ServiceProvider.GetRequiredService<IProductSaleRepository>();
            var productSale = new ProductSale(Guid.NewGuid(), productId, (ProductSaleState)random.Next(0, count), endPrice, email, additionId, orderId);
            await productSaleRepository.AddAsync(productSale);
            return productSale;
        }

        public async Task<Order> AddDefaultOrderAsync(Email email, IEnumerable<ProductSale>? productSales = null)
        {
            productSales ??= new List<ProductSale>();
            Scope ??= Fixture.Services.CreateScope();
            var orderRepository = Scope.ServiceProvider.GetRequiredService<IOrderRepository>();
            var productSaleRepository = Scope.ServiceProvider.GetRequiredService<IProductSaleRepository>();
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

        public async Task<User> AddDefaultUserAsync(Email email)
        {
            Scope ??= Fixture.Services.CreateScope();
            var user = new User(Guid.NewGuid(), email, "DefaultPaswword", User.Roles.UserRole, DateTime.UtcNow);
            var userRepository = Scope.ServiceProvider.GetRequiredService<IUserRepository>();
            await userRepository.AddAsync(user);
            return user;
        }
    }
}
