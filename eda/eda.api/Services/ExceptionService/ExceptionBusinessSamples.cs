using MassTransit;
using Serilog;

namespace eda.api.Services.ExceptionService
{
    public class ExceptionBusinessSamples : IConsumer<ExceptionCommand>
    {
        public async Task Consume(ConsumeContext<ExceptionCommand> context)
        {
            Log.Error("invoked");

            if (context.Message.Text == "exception")
            {
                throw new DatabaseQueryTimeoutException("Database query timeout.");
            }
            else if (context.Message.Text == "0")
            {
                throw new DivideByZeroException();
            }
            Log.Error("no exceptions");
        }
    }

    public class DatabaseQueryTimeoutException : Exception
    {
        public DatabaseQueryTimeoutException(string message) : base(message) { }
    }
}
