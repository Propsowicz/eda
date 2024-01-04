using csStorage.Builder.csContextBuilder;
using eda.api.Messages.Commands;
using eda.api.Messages.Events;
using eda.api.Services.VisitService.Models;
using MassTransit;
using MassTransit.Mediator;

namespace eda.api.Services.PaymentService
{
    public class PaymentExecutionCheckBusiness : IConsumer<CheckIfPaymentExecuted>
    {
        private readonly csContextBuilder<PaymentEntity> _contextPaymentEntityBuilder;
        private readonly IMediator _mediator;

        public PaymentExecutionCheckBusiness(IMediator mediator)
        {
            _mediator = mediator;
            _contextPaymentEntityBuilder = new csContextBuilder<PaymentEntity>();
        }

        public async Task Consume(ConsumeContext<CheckIfPaymentExecuted> context)
        {
            var paymentEntity = _contextPaymentEntityBuilder.Get().First(x => x.Id == context.Message.PaymentId);

            if (!paymentEntity.IsPayed)
            {
                var visitModel = await _mediator.SendRequest(new VisitQueryRequestModel { Id = paymentEntity.VisitId });
                await CancelVisitCommand(context, visitModel.Id);

                await PublishVisitCancelledEvent(context, visitModel);
                await SendVisitCancelledCommand(context, visitModel);
            }
        }

        private static async Task CancelVisitCommand(ConsumeContext<CheckIfPaymentExecuted> context, Guid visitId)
        {
            var sendEndpoint =
                await context.GetSendEndpoint(new Uri("queue:cancel_visit_command"));
            await sendEndpoint.Send<CancelVisitCommand>(new
            {
                VisitId = visitId,
                CancellationReason = CancellationReason.PaymentExpiration
            });
        }

        private static async Task SendVisitCancelledCommand(ConsumeContext<CheckIfPaymentExecuted> context, VisitQueryResponseModel visitModel)
        {
            var sendEndpoint =
                await context.GetSendEndpoint(new Uri("queue:send_visit_cancelled_email_command"));
            await sendEndpoint.Send<SendVisitCancelledEmail>(new
            {
                CancellationReason = CancellationReason.PaymentExpiration,
                VisitDate = visitModel.VisitDate,
                PatientName = visitModel.PatientName,
                PatientEmail = visitModel.PatientEmail,
                DoctorName = visitModel.DoctorName,
                HospitalName = visitModel.HospitalName
            });
        }

        private static async Task PublishVisitCancelledEvent(ConsumeContext<CheckIfPaymentExecuted> context, VisitQueryResponseModel visitModel)
        {
            await context.Publish<VisitCancelled>(new
            {
                CancellationReason = CancellationReason.PaymentExpiration,
                VisitDate = visitModel.VisitDate,
                PatientName = visitModel.PatientName,
                DoctorName = visitModel.DoctorName,
                HospitalName = visitModel.HospitalName
            });
        }
    }
}
