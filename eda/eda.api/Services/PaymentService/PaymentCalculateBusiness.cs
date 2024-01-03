using csStorage.Builder.csContextBuilder;
using eda.api.Messages.Commands;
using eda.api.Services.VisitService;
using MassTransit;

namespace eda.api.Services.PaymentService
{
    public class PaymentCalculateBusiness : IConsumer<CalculateVisitBill>
    {
        private readonly int _expirationSeconds = 60;

        private readonly csContextBuilder<VisitEntity> _contextVisitEntityBuilder;
        private readonly csContextBuilder<PaymentEntity> _contextBillEntityBuilder;

        public PaymentCalculateBusiness()
        {
            _contextVisitEntityBuilder = new csContextBuilder<VisitEntity>();
            _contextBillEntityBuilder = new csContextBuilder<PaymentEntity>();
        }

        public async Task Consume(ConsumeContext<CalculateVisitBill> context)
        {
            var visitEntity = _contextVisitEntityBuilder.Get().First(x => x.Id == context.Message.VisitId);
            var visitPrice = CalculatePrice();
            var dateOfPayment = DateTime.UtcNow.AddSeconds(_expirationSeconds);

            var billEntity = new PaymentEntity
            {
                Id = Guid.NewGuid(),
                VisitId = visitEntity.Id,
                VisitPrice = visitPrice,
                DateOfPayment = dateOfPayment,
                IsPayed = false
            };
            await _contextBillEntityBuilder.AddAsync(billEntity);

            await SendVisitRegisteredReturnEmailCommand(context, visitEntity, billEntity);
            await SendPaymentExpirationCheckCommand(context, billEntity);
        }

        private static async Task SendPaymentExpirationCheckCommand(ConsumeContext<CalculateVisitBill> context, PaymentEntity paymentEntity)
        {
            await context.ScheduleSend<CheckIfPaymentIsDone>(new Uri("queue:send_send_payment_expiration_check_command"), paymentEntity.DateOfPayment, new
            {
                BillId = paymentEntity.Id
            });
        }

        private static async Task SendVisitRegisteredReturnEmailCommand(ConsumeContext<CalculateVisitBill> context, VisitEntity visitEntity,
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
                    Bill = paymentEntity.VisitPrice
                });
        }

        private double CalculatePrice()
        {
            var visitPrice = new Random().Next(100, 250);
            var hospitalCost = 100;
            var taxPercentage = 1.23;
            return (visitPrice + hospitalCost) * taxPercentage;
        }
    }
}
