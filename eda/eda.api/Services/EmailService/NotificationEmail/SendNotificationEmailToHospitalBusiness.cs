using eda.api.Messages.Events;
using eda.api.Services.EmailService;
using MassTransit;

namespace eda.api.Services.EmailService.NotificationEmail
{
    public class SendNotificationEmailToHospitalBusiness : BaseNotificationEmailBusiness
    {
        public SendNotificationEmailToHospitalBusiness(IEmailService emailService) : base(emailService) { }

        public override string VisitRegisteredEmailMessage(ConsumeContext<NewVisitRegistered> context)
        {
            return $"New visit has been registered. Patient {context.Message.PatientName} visits {context.Message.HospitalName} hospital on {context.Message.VisitDate} to meet with {context.Message.DoctorName}.";
        }

        public override string GetToEmail(ConsumeContext<NewVisitRegistered> context)
        {
            return $"{context.Message.HospitalName}@visit.com";
        }
    }
}
