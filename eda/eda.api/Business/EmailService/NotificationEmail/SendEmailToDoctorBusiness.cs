using eda.api.Business.EmailService.NotificationEmail;
using eda.api.EventModels;
using MassTransit;

namespace eda.api.Business.EmailService
{
    public class SendEmailToDoctorBusiness : BaseNotificationEmailBusiness
    {
        public SendEmailToDoctorBusiness(IEmailService emailService) : base(emailService)
        {
        }

        public override string CreateMessage(ConsumeContext<VisitRegisteredPublishModel> context)
        {
            return "add table with doctors nad their emails to fetch it here";
        }

        public override string GetToEmail(ConsumeContext<VisitRegisteredPublishModel> context)
        {
            return context.Message.CorrelationId.ToString();
        }
    }
}
