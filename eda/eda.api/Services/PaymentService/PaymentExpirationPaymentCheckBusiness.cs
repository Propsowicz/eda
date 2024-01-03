using csStorage.Builder.csContextBuilder;
using eda.api.Messages.Commands;
using eda.api.Messages.Events;
using eda.api.Services.VisitService;
using MassTransit;

namespace eda.api.Services.PaymentService
{
    public class PaymentExpirationPaymentCheckBusiness : IConsumer<CheckIfPaymentIsDone>
    {
        private readonly csContextBuilder<VisitEntity> _contextVisitEntityBuilder;
        private readonly csContextBuilder<PaymentEntity> _contextPaymentEntityBuilder;

        public PaymentExpirationPaymentCheckBusiness()
        {
            _contextVisitEntityBuilder = new csContextBuilder<VisitEntity>();
            _contextPaymentEntityBuilder = new csContextBuilder<PaymentEntity>();
        }

        public async Task Consume(ConsumeContext<CheckIfPaymentIsDone> context)
        {
            var paymentEntity = _contextPaymentEntityBuilder.Get().First(x => x.Id == context.Message.PaymentId);

            if (!paymentEntity.IsPayed)
            {
                var visitEntity = _contextVisitEntityBuilder.Get().First(x => x.Id == paymentEntity.VisitId);
                visitEntity.IsCancelled = true;

                await _contextVisitEntityBuilder.UpdateAsync(visitEntity);

                await PublishVisitCancelledEvent(context, visitEntity);
                await SendVisitCancelledCommand(context, visitEntity);
            }
        }

        private static async Task SendVisitCancelledCommand(ConsumeContext<CheckIfPaymentIsDone> context, VisitEntity visitEntity)
        {
            var sendEndpoint =
                await context.GetSendEndpoint(new Uri("queue:send_visit_cancelled_email_command"));
            await sendEndpoint.Send<SendPaymentExpirationEmail>(new
            {
                CancellationReason = CancellationReason.PaymentExpiration,
                visitEntity.VisitDate,
                visitEntity.PatientName,
                visitEntity.DoctorName,
                visitEntity.HospitalName
            });
        }

        private static async Task PublishVisitCancelledEvent(ConsumeContext<CheckIfPaymentIsDone> context, VisitEntity visitEntity)
        {
            await context.Publish<VisitCancelled>(new
            {
                CancellationReason = CancellationReason.PaymentExpiration,
                visitEntity.VisitDate,
                visitEntity.PatientName,
                visitEntity.DoctorName,
                visitEntity.HospitalName
            });
        }
    }
}
