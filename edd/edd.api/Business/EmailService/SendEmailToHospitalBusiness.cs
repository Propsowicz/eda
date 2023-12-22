using edd.api.EventModels;
using MassTransit;
using Serilog;

namespace edd.api.Business.EmailService
{
    public class SendEmailToHospitalBusiness : SendEmailBusinessBase, IConsumer<VisitRegisteredPublishModel>
    {
        public async Task Consume(ConsumeContext<VisitRegisteredPublishModel> context)
        {
            await SendEmailAsync("hospitals@visit.com", context.Message.Message);
        }
    }
}
