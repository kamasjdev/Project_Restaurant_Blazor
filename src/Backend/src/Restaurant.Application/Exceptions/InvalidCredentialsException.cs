namespace Restaurant.Application.Exceptions
{
    public sealed class InvalidCredentialsException : BusinessException
    {
        public InvalidCredentialsException() : base("Invalid credentials")
        {
        }
    }
}
