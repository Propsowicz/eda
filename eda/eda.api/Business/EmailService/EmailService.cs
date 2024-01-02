using Serilog;

namespace eda.api.Business.EmailService
{
    public class EmailService : IEmailService
    {
        public void SendEmail(string toEmail, string msg)
        {
            var fromEmail = "system@visit.com";
            Log.Error($"-----------------------");
            Log.Error($"To: {toEmail}");
            Log.Error($"From: {fromEmail}");
            Log.Error($"{msg}");
            Log.Error($"-----------------------");
        }
    }
}
