using Restaurant.Core.Exceptions;
using Restaurant.Core.ValueObjects;

namespace Restaurant.Core.Entities
{
    public class Product
    {
        public EntityId Id { get; }
        public ProductName ProductName { get; private set; }
        public Price Price { get; private set; }
        public ProductKind ProductKind { get; private set; }

        public IEnumerable<Order> Orders => _orders;
        private IList<Order> _orders = new List<Order>();

        private IList<EntityId> _productSaleIds = new List<EntityId>();
        public IEnumerable<EntityId> ProductSaleIds => _productSaleIds;

        public Product(EntityId id, ProductName productName, Price price, ProductKind productKind, IEnumerable<Order>? orders = null, IEnumerable<EntityId>? productSaleIds = null)
        {
            Id = id;
            ChangeProductName(productName);
            ChangePrice(price);
            _orders = orders?.ToList() ?? new List<Order>();
            ProductKind = productKind;
            _productSaleIds = productSaleIds?.ToList() ?? new List<EntityId>();
        }

        public Product(EntityId id, ProductName productName, Price price, string productKind, IEnumerable<Order>? orders = null, IEnumerable<EntityId>? productSaleIds = null)
        {
            Id = id;
            ChangeProductName(productName);
            ChangePrice(price);
            _orders = orders?.ToList() ?? new List<Order>();
            ProductKind = ParseToProductKind(productKind);
            _productSaleIds = productSaleIds?.ToList() ?? new List<EntityId>();
        }

        public void ChangePrice(Price price)
        {
            Price = price;
        }

        public void ChangeProductName(ProductName productName)
        {
            ProductName = productName;
        }

        public void ChangeProductKind(ProductKind productKind)
        {
            ProductKind = productKind;
        }

        public void ChangeProductKind(string productKind)
        {
            ProductKind = ParseToProductKind(productKind);
        }

        public void AddOrders(IEnumerable<Order>? orders)
        {
            if (orders is null)
            {
                throw new OrderCannotBeNullException();
            }

            foreach (var order in orders)
            {
                _orders.Add(order);
            }
        }

        public void AddOrder(Order? order)
        {
            if (order is null)
            {
                throw new OrderCannotBeNullException();
            }

            var orderToAdd = _orders.Where(p => p.Id == order.Id).SingleOrDefault();

            if (orderToAdd != null)
            {
                throw new OrderAlreadyExistsException(orderToAdd.Id);
            }

            _orders.Add(order);
        }

        private static ProductKind ParseToProductKind(string productKind)
        {
            var parsed = Enum.TryParse<ProductKind>(productKind, out var productKindParsed);

            if (!parsed)
            {
                throw new InvalidProductKindException(productKind);
            }

            if (!Enum.IsDefined(productKindParsed))
            {
                throw new InvalidProductKindException(productKind);
            }

            return productKindParsed;
        }
    }
}
