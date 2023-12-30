using edd.api.EventModels;
using MassTransit;

namespace edd.api.Business.EmailService.NotificationEmail
{
    public abstract class BaseNotificationEmailBusiness : IConsumer<VisitRegisteredPublishModel>
    {
        private readonly IEmailService _emailService;

        protected BaseNotificationEmailBusiness(IEmailService emailService)
        {
            _emailService = emailService;
        }

        public async Task Consume(ConsumeContext<VisitRegisteredPublishModel> context)
        {
            _emailService.SendEmail(GetToEmail(context), CreateMessage(context));
        }

        public abstract string CreateMessage(ConsumeContext<VisitRegisteredPublishModel> context);
        public abstract string GetToEmail(ConsumeContext<VisitRegisteredPublishModel> context);
    }
}
