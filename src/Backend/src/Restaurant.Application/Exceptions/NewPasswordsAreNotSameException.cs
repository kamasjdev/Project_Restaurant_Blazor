namespace Restaurant.Application.Exceptions
{
    public sealed class NewPasswordsAreNotSameException : ApplicationException
    {
        public NewPasswordsAreNotSameException() : base("New passwords are not same")
        {
        }
    }
}
