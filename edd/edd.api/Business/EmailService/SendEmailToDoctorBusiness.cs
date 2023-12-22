using edd.api.EventModels;
using MassTransit;
using Serilog;

namespace edd.api.Business.EmailService
{
    public class SendEmailToDoctorBusiness : SendEmailBusinessBase, IConsumer<VisitRegisteredPublishModel>
    {
        public async Task Consume(ConsumeContext<VisitRegisteredPublishModel> context)
        {
            await SendEmailAsync("doctors@visit.com", context.Message.Message);
        }
    }
}
