namespace Restaurant.Core.Exceptions
{
    public class PriceCannotBeNullException : DomainException
    {
        public PriceCannotBeNullException() : base("Price cannot be null")
        {
        }
    }
}
