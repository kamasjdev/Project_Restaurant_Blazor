namespace Restaurant.Application.Exceptions
{
    public sealed class CannotConstructEmailFromOrderException : ApplicationException
    {
        public CannotConstructEmailFromOrderException() : base("Cannot construct email from null order")
        {
        }
    }
}
