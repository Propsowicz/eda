using csStorage.Builder.csContextBuilder;
using eda.api.Messages.Commands;
using eda.api.Services.VisitService.Models;
using MassTransit;
using MassTransit.Mediator;

namespace eda.api.Services.PaymentService
{
    public class PaymentCalculateBusiness : IConsumer<CalculateVisitPrice>
    {
        private readonly int _expirationSeconds = 60;

        private readonly csContextBuilder<PaymentEntity> _contextPaymentEntityBuilder;
        private readonly IMediator _mediator;

        public PaymentCalculateBusiness(IMediator mediator)
        {
            _mediator = mediator;
            _contextPaymentEntityBuilder = new csContextBuilder<PaymentEntity>();
        }

        public async Task Consume(ConsumeContext<CalculateVisitPrice> context)
        {
            var visitModel = await _mediator.SendRequest(new VisitQueryRequestModel { Id = context.Message.VisitId });
            var visitPrice = CalculatePrice();
            var dateOfPayment = DateTime.UtcNow.AddSeconds(_expirationSeconds);

            var paymentEntity = new PaymentEntity
            {
                Id = Guid.NewGuid(),
                VisitId = visitModel.Id,
                VisitPrice = visitPrice,
                DateOfPayment = dateOfPayment,
                IsPayed = false
            };
            await _contextPaymentEntityBuilder.AddAsync(paymentEntity);

            await SendVisitRegisteredReturnEmailCommand(context, visitModel, paymentEntity);
            await SendPaymentExpirationCheckCommand(context, paymentEntity);
        }

        private static async Task SendPaymentExpirationCheckCommand(ConsumeContext<CalculateVisitPrice> context, PaymentEntity paymentEntity)
        {
            await context.ScheduleSend<CheckIfPaymentExecuted>(new Uri("queue:check_if_payment_executed_command"), paymentEntity.DateOfPayment, new
            {
                CorrelationId = context.Message.CorrelationId,
                PaymentId = paymentEntity.Id
            });
        }

        private static async Task SendVisitRegisteredReturnEmailCommand(ConsumeContext<CalculateVisitPrice> context, VisitQueryResponseModel visitModel,
            PaymentEntity paymentEntity)
        {
            await context.Send<SendReturnEmail>(new Uri("queue:send_visit_registered_return_email_command"),
                new
                {
                    PaymentId = paymentEntity.Id,
                    visitModel.PatientEmail,
                    visitModel.VisitDate,
                    visitModel.HospitalName,
                    DayOfPayment = paymentEntity.DateOfPayment,
                    VisitPrice = paymentEntity.VisitPrice
                });
        }

        private double CalculatePrice()
        {
            var visitPrice = new Random().Next(100, 250);
            var hospitalCost = 100;
            var taxPercentage = 1.23;
            return Math.Round(((visitPrice + hospitalCost) * taxPercentage), 2) ;
        }
    }
}
