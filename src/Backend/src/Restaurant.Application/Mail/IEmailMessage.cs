namespace Restaurant.Application.Mail
{
    public interface IEmailMessage
    {
        public string Subject { get; }
        public string Body { get; }
    }
}
