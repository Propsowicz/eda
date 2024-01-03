using csStorage.Builder.csContextBuilder;
using eda.api.Messages.Commands;
using eda.api.Services.VisitService;
using MassTransit;

namespace eda.api.Services.PaymentService
{
    public class PaymentCalculateBusiness : IConsumer<CalculateVisitPrice>
    {
        private readonly int _expirationSeconds = 60;

        private readonly csContextBuilder<VisitEntity> _contextVisitEntityBuilder;
        private readonly csContextBuilder<PaymentEntity> _contextPaymentEntityBuilder;

        public PaymentCalculateBusiness()
        {
            _contextVisitEntityBuilder = new csContextBuilder<VisitEntity>();
            _contextPaymentEntityBuilder = new csContextBuilder<PaymentEntity>();
        }

        public async Task Consume(ConsumeContext<CalculateVisitPrice> context)
        {
            var visitEntity = _contextVisitEntityBuilder.Get().First(x => x.Id == context.Message.VisitId);
            var visitPrice = CalculatePrice();
            var dateOfPayment = DateTime.UtcNow.AddSeconds(_expirationSeconds);

            var paymentEntity = new PaymentEntity
            {
                Id = Guid.NewGuid(),
                VisitId = visitEntity.Id,
                VisitPrice = visitPrice,
                DateOfPayment = dateOfPayment,
                IsPayed = false
            };
            await _contextPaymentEntityBuilder.AddAsync(paymentEntity);

            await SendVisitRegisteredReturnEmailCommand(context, visitEntity, paymentEntity);
            await SendPaymentExpirationCheckCommand(context, paymentEntity);
        }

        private static async Task SendPaymentExpirationCheckCommand(ConsumeContext<CalculateVisitPrice> context, PaymentEntity paymentEntity)
        {
            await context.ScheduleSend<CheckIfPaymentIsDone>(new Uri("queue:check_if_payment_is_done_command"), paymentEntity.DateOfPayment, new
            {
                PaymentId = paymentEntity.Id
            });
        }

        private static async Task SendVisitRegisteredReturnEmailCommand(ConsumeContext<CalculateVisitPrice> context, VisitEntity visitEntity,
            PaymentEntity paymentEntity)
        {
            await context.Send<SendReturnEmail>(new Uri("queue:send_visit_registered_return_email_command"),
                new
                {
                    PaymentId = paymentEntity.Id,
                    visitEntity.PatientEmail,
                    visitEntity.VisitDate,
                    visitEntity.HospitalName,
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
