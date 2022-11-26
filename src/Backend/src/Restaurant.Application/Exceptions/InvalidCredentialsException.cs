namespace Restaurant.Application.Exceptions
{
    public sealed class InvalidCredentialsException : ApplicationException
    {
        public InvalidCredentialsException() : base("Invalid credentials")
        {
        }
    }
}
