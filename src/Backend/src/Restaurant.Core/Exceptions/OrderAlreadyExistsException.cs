using Restaurant.Core.ValueObjects;

namespace Restaurant.Core.Exceptions
{
    public sealed class OrderAlreadyExistsException : DomainException
    {
        public EntityId OrderId { get; }

        public OrderAlreadyExistsException(EntityId orderId) : base($"Order with id: '{orderId}' already exists")
        {
            OrderId = orderId;
        }
    }
}
