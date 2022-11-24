namespace Restaurant.Core.Exceptions
{
    public sealed class OrderNumberCannotBeEmptyException : DomainException
    {
        public OrderNumberCannotBeEmptyException() : base("OrderNumber cannot be empty")
        {
        }
    }
}
