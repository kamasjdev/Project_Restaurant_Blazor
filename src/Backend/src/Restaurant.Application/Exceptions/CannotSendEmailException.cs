namespace Restaurant.Application.Exceptions
{
    public sealed class CannotSendEmailException : BusinessException
    {
        public CannotSendEmailException() : base("Mail can't be sent. Probably invalid settings, please fill properly")
        {
        }
    }
}
