using eda.api.Messages.Commands;
using eda.api.Services.EmailService;
using MassTransit;

namespace eda.api.Services.EmailService.EmailToPatient;

public class SendEmailToPatientBusiness : IConsumer<SendReturnEmail>, IConsumer<SendVisitCancelledEmail>
{
    private readonly IEmailService _emailService;

    public SendEmailToPatientBusiness(IEmailService emailService)
    {
        _emailService = emailService;
    }

    public async Task Consume(ConsumeContext<SendReturnEmail> context)
    {
        var toEmail = context.Message.PatientEmail;
        var msg = $"Your visit has been registered. Visit date: {context.Message.VisitDate}. Visit place: {context.Message.HospitalName}. Your payment is {context.Message.VisitPrice}$ - no. of payment {context.Message.PaymentId}. You need to pay the bill till the {context.Message.DayOfPayment}, otherwise the visit gonna be cancelled.";

        _emailService.SendEmail(toEmail, msg);
    }

    public async Task Consume(ConsumeContext<SendVisitCancelledEmail> context)
    {
        var toEmail = context.Message.PatientEmail;
        var msg = $"Your visit has been cancelled due to the {context.Message.CancellationReason}.";

        _emailService.SendEmail(toEmail, msg);
    }
}