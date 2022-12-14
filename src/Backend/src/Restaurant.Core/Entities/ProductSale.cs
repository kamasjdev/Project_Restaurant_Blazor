using Restaurant.Core.Exceptions;
using Restaurant.Core.ValueObjects;

namespace Restaurant.Core.Entities
{
    public sealed class ProductSale : BaseEntity
    {
        public EntityId ProductId { get; private set; }
        public Product Product { get; private set; }
        public EntityId? AdditionId { get; private set; } = null;
        public Addition? Addition { get; private set; } = null;
        public Price EndPrice { get; private set; } = decimal.Zero;
        public EntityId? OrderId { get; private set; }
        public Order? Order { get; private set; } = null;
        public ProductSaleState ProductSaleState { get; private set; } = ProductSaleState.New;
        public Email Email { get; private set; }

        public ProductSale(EntityId? id, Product? product, ProductSaleState productSaleState, Email email, Addition? addition = null, Order? order = null)
            : base(id)
        {
            ChangeProduct(product);

            if (addition is not null)
            {
                ChangeAddition(addition);
            }

            ProductSaleState = productSaleState;
            Email = email;

            if (order is not null)
            {
                AddOrder(order);
            }
        }

        public ProductSale(EntityId? id, EntityId productId, ProductSaleState productSaleState, decimal endPrice, Email email, EntityId? additionId = null, EntityId? orderId = null)
            : base(id)
        {
            ProductId = productId;
            OrderId = orderId;
            AdditionId = additionId;
            ProductSaleState = productSaleState;
            EndPrice = endPrice;
            Email = email;
        }

        public void ChangeProduct(Product? product)
        {
            if (product is null)
            {
                throw new ProductCannotBeNullException();
            }

            if (Product != null)
            {
                EndPrice -= Product.Price;
            }

            Product = product;
            ProductId = product.Id;
            EndPrice += product.Price;
        }

        public void ChangeAddition(Addition? addition)
        {
            if (addition is null)
            {
                throw new AdditionCannotBeNullException();
            }

            if (Addition != null)
            {
                EndPrice -= Addition.Price;
            }

            Addition = addition;
            AdditionId = addition.Id;
            EndPrice += addition.Price;
        }

        public void RemoveAddition()
        {
            if (Addition is null)
            {
                throw new AdditionNotExistsException(Id);
            }

            EndPrice -= Addition.Price;
            Addition = null;
            AdditionId = null;
        }

        public void AddOrder(Order? order)
        {
            if (order is null)
            {
                throw new OrderCannotBeNullException();
            }

            if (Order != null)
            {
                throw new CannotOverrideExistingOrderException(Id);
            }

            Order = order;
            OrderId = order.Id;
            ProductSaleState = ProductSaleState.Ordered;
        }

        public void RemoveOrder()
        {
            if (Order is null)
            {
                throw new OrderNotExsitsException(Id);
            }

            Order = null;
            OrderId = null;
            ProductSaleState = ProductSaleState.New;
        }

        public void ChangeEmail(Email email)
        {
            Email = email;
        }
    }
}
