using eda.api.Messages.Events;
using MassTransit;

namespace eda.api.Services.EmailService.NotificationEmail
{
    public abstract class BaseNotificationEmailBusiness : IConsumer<NewVisitRegistered>, IConsumer<VisitCancelled>
    {
        private readonly IEmailService _emailService;

        protected BaseNotificationEmailBusiness(IEmailService emailService)
        {
            _emailService = emailService;
        }

        public async Task Consume(ConsumeContext<NewVisitRegistered> context)
        {
            _emailService.SendEmail(GetToEmail(context), VisitRegisteredEmailMessage(context));
        }
        public abstract string VisitRegisteredEmailMessage(ConsumeContext<NewVisitRegistered> context);
        public abstract string GetToEmail(ConsumeContext<NewVisitRegistered> context);

        public async Task Consume(ConsumeContext<VisitCancelled> context)
        {
            _emailService.SendEmail(GetToEmail(context), VisitCancelledMessage(context));
        }
        public string VisitCancelledMessage(ConsumeContext<VisitCancelled> context)
        {
            return $"Visit of patient {context.Message.PatientName} in {context.Message.HospitalName} hospital on {context.Message.VisitDate} has been cancelled due to the {context.Message.CancellationReason}.";
        }
        public string GetToEmail(ConsumeContext<VisitCancelled> context)
        {
            return MockedEmailList.Get().First(x => x.Name == context.Message.DoctorName).Email;
        }
    }
}
