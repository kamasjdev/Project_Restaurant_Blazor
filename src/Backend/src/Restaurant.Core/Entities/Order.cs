using Restaurant.Core.Exceptions;
using Restaurant.Core.ValueObjects;

namespace Restaurant.Core.Entities
{
    public class Order
    {
        public EntityId Id { get; }
        public OrderNumber OrderNumber { get; private set; }
        public DateTime Created { get; }
        public Price Price { get; private set; }
        public Email Email { get; private set; }
        public string? Note { get; private set; } = null;
        public IEnumerable<ProductSale> Products => _products;
        private IList<ProductSale> _products = new List<ProductSale>();

        public Order(EntityId id, OrderNumber orderNumber, DateTime created, Price price, Email email, string? note = null, IEnumerable<ProductSale>? products = null)
        {
            Id = id;
            ChangeOrderNumber(orderNumber);
            Created = created;
            ChangePrice(price);
            ChangeEmail(email);

            if (products != null)
            {
                AddProducts(products);
            }

            Note = note;
        }

        public void ChangeOrderNumber(string orderNumber)
        {
            OrderNumber = orderNumber;
        }

        public void ChangePrice(decimal price)
        {
            Price = price;
        }

        public void ChangeEmail(Email email)
        {
            Email = email;
        }

        public void ChangeNote(string? note)
        {
            Note = note;
        }

        public void AddProducts(IEnumerable<ProductSale>? products)
        {
            if (products is null)
            {
                throw new ProductCannotBeNullException();
            }

            foreach (var product in products)
            {
                AddProduct(product);
            }
        }

        public void AddProduct(ProductSale? product)
        {
            if (product is null)
            {
                throw new ProductCannotBeNullException();
            }

            var productToAdd = _products.Where(p => p.Id == product.Id).SingleOrDefault();

            if (productToAdd != null)
            {
                throw new ProductAlreadyExistsException(productToAdd.Id);
            }

            _products.Add(product);
            product.AddOrder(this);
        }

        public void RemoveProduct(ProductSale? product)
        {
            if (product is null)
            {
                throw new ProductCannotBeNullException();
            }

            var productToDelete = _products.Where(p => p.Id == product.Id).SingleOrDefault();

            if (productToDelete is null)
            {
                throw new ProductNotFoundException(product.Id);
            }

            _products.Remove(product);
            product.RemoveOrder();
        }
    }
}
