namespace eda.api.Services.EmailService
{
    public interface IEmailService
    {
        public void SendEmail(string toEmail, string msg);
    }
}
