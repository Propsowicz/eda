using System.Text;
using Serilog;

namespace eda.api.Services.EmailService
{
    public class EmailService : IEmailService
    {
        private readonly string _EmailSender = "system@visit.com";

        public void SendEmail(string toEmail, string msg)
        {
            var sb = new StringBuilder();
            sb.Append("\n-----------------------\n");
            sb.Append($"To: {toEmail}\n");
            sb.Append($"From: {_EmailSender}\n");
            sb.Append($"{msg}\n");
            sb.Append("-----------------------\n");

            Log.Error(sb.ToString());
        }
    }
}
