namespace Restaurant.Application.Exceptions
{
    public sealed class CannotDeleteProductOrderedException : BusinessException
    {
        public Guid ProductId { get; }

        public CannotDeleteProductOrderedException(Guid productId) : base($"Cannot delete Product ordered with id:'{productId}'")
        {
            ProductId = productId;
        }
    }
}
