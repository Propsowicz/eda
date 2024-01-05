using MassTransit;
using Serilog;
using System.Text;

namespace eda.api.Services.ExceptionService
{
    public class ExceptionsConsumer : IConsumer<Fault<ExceptionCommand>>
    {
        public async Task Consume(ConsumeContext<Fault<ExceptionCommand>> context)
        {
            var sb = new StringBuilder();
            sb.Append("\nException happened!\n");
            sb.Append($"CorrelationId: {context.Message.Message.CorrelationId}\n");
            sb.Append($"Text: {context.Message.Message.Text}\n");
            sb.Append($"Timestamp: {context.Message.Timestamp}\n");

            foreach (var exception in context.Message.Exceptions)
            {
                sb.Append($"Exception: {exception.Message}\n");
            }

            sb.Append($"SourceAddress: {context.SourceAddress}\n");
            sb.Append($"DestinationAddress: {context.DestinationAddress}\n");

            Log.Error(sb.ToString());
        }
    }
}
