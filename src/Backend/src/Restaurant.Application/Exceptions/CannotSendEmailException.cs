namespace Restaurant.Application.Exceptions
{
    public sealed class CannotSendEmailException : ApplicationException
    {
        public CannotSendEmailException() : base("Mail can't be sent. Probably invalid settings, please fill properly")
        {
        }
    }
}
