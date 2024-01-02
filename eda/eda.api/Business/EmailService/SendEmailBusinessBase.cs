using Serilog;

namespace eda.api.Business.EmailService
{
    public abstract class SendEmailBusinessBase
    {
        public async Task SendEmailAsync(string destinationEmail, string msg)
        {
            Log.Error($"-----------------------");
            Log.Error($"To: {destinationEmail}");
            Log.Error($"{msg}");
            Log.Error($"-----------------------");
        }
    }
}
