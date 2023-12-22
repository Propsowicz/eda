using edd.api.EventModels;
using MassTransit;

namespace edd.api.Business.EmailService
{
    public class SendReturnEmailBusiness : SendEmailBusinessBase, IConsumer<VisitRegisteredSendModel>
    {
        public async Task Consume(ConsumeContext<VisitRegisteredSendModel> context)
        {
            var msg = $"Hello! You have been registered a visit in our {context.Message.VisitEntity.HospitalName} hospital on {context.Message.VisitEntity.HospitalName}. Price for the visit: {context.Message.PriceAfterTax}$";
            await SendEmailAsync(context.Message.VisitEntity.PatientEmail, msg);
        }
    }
}
