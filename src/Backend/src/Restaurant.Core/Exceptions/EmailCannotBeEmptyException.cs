namespace Restaurant.Core.Exceptions
{
    public sealed class EmailCannotBeEmptyException : DomainException
    {
        public EmailCannotBeEmptyException() : base("Email cannot be empty")
        {
        }
    }
}
