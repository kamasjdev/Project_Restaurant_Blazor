namespace Restaurant.Application.Exceptions
{
    public sealed class CannotDeleteProductOrderedException : ApplicationException
    {
        public Guid ProductId { get; }

        public CannotDeleteProductOrderedException(Guid productId) : base($"Cannot delete Product ordered with id:'{productId}'")
        {
            ProductId = productId;
        }
    }
}
