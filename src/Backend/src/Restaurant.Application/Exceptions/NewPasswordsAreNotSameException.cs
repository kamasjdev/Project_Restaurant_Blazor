namespace Restaurant.Application.Exceptions
{
    public sealed class NewPasswordsAreNotSameException : BusinessException
    {
        public NewPasswordsAreNotSameException() : base("New passwords are not same")
        {
        }
    }
}
