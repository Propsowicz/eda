using eda.api.Business.EmailService.NotificationEmail;
using eda.api.EventModels;
using MassTransit;
using Serilog;

namespace eda.api.Business.EmailService
{
    public class SendEmailToHospitalBusiness : BaseNotificationEmailBusiness
    {
        public SendEmailToHospitalBusiness(IEmailService emailService) : base(emailService)
        {
        }

        public override string CreateMessage(ConsumeContext<VisitRegisteredPublishModel> context)
        {
            return "TODO - change event types (0more in eda style - as small as possible and as precise as possible)";
        }

        public override string GetToEmail(ConsumeContext<VisitRegisteredPublishModel> context)
        {
            return context.Message.CorrelationId.ToString();
        }
    }
}
