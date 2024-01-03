using eda.api.Messages.Events;
using eda.api.Services.EmailService;
using MassTransit;

namespace eda.api.Services.EmailService.NotificationEmail
{
    public class SendNotificationEmailToDoctorBusiness : BaseNotificationEmailBusiness
    {
        public SendNotificationEmailToDoctorBusiness(IEmailService emailService) : base(emailService) { }

        public override string VisitRegisteredEmailMessage(ConsumeContext<NewVisitRegistered> context)
        {
            return $"New visit has been registered. Patient {context.Message.PatientName} visits you in {context.Message.HospitalName} hospital on {context.Message.VisitDate}.";
        }

        public override string GetToEmail(ConsumeContext<NewVisitRegistered> context)
        {
            return MockedEmailList.Get().First(x => x.Name == context.Message.DoctorName).Email;
        }
    }
}
