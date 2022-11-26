namespace Restaurant.Application.Mail
{
    public class EmailSettings
    {
        public bool SendEmailAfterConfirmOrder { get; set; }
        public string? Login { get; set; }
        public string? Password { get; set; }
        public string? SmtpClient { get; set; }
        public int SmtpPort { get; set; }
        public string? Email { get; set; }
        public int Timeout { get; set; }
    }
}
