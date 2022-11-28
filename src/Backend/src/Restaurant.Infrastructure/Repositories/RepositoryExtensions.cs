using Restaurant.Core.Entities;
using Restaurant.Core.ValueObjects;
using Restaurant.Infrastructure.Repositories.DBO;
using System.Reflection;

namespace Restaurant.Infrastructure.Repositories
{
    internal static class RepositoryExtensions
    {
        public static Product AsDetailsEntity(this ProductDBO productData)
        {
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

        public static ProductSale AsDetailsEntity(this ProductSaleDBO productSaleData)
        {
            var productSaleIds = new List<EntityId> { new EntityId(productSaleData.Id) };
            Product product = new(productSaleData.Product.Id, productSaleData.Product.ProductName, productSaleData.Product.Price, productSaleData.Product.ProductKind, null, productSaleIds);
            Addition? addition = productSaleData.Addition is not null ? new Addition(productSaleData.Addition.Id, productSaleData.Addition.AdditionName, productSaleData.Addition.Price, productSaleData.Addition.AdditionKind, productSaleIds) : null;
            ProductSale productSale = new(productSaleData.Id, product, productSaleData.ProductSaleState, Email.Of(productSaleData.Email), addition);
            
            if (productSaleData.Order is not null) {
                _ = new Order(productSaleData.Order.Id, productSaleData.Order.OrderNumber, productSaleData.Order.Created, productSaleData.Order.Price, Email.Of(productSaleData.Order.Email), productSaleData.Order.Note, new List<ProductSale> { productSale });
            }

            return productSale;
        }

        public static Order AsDetailsEntity(this OrderDBO orderData)
        {
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
    }
}
