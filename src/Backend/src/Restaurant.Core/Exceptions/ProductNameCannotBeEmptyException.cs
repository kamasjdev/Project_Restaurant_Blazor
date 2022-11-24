namespace Restaurant.Core.Exceptions
{
    public sealed class ProductNameCannotBeEmptyException : DomainException
    {
        public ProductNameCannotBeEmptyException() : base("ProductName cannot be empty")
        {
        }
    }
}
