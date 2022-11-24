namespace Restaurant.Core.Exceptions
{
    public sealed class ProductCannotBeNullException : DomainException
    {
        public ProductCannotBeNullException() : base("Product cannot be null")
        {
        }
    }
}
