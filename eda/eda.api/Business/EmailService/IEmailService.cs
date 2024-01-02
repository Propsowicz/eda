namespace eda.api.Business.EmailService
{
    public interface IEmailService
    {
        public void SendEmail(string toEmail, string msg);
    }
}
