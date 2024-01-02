using eda.api.EventModels;
using MassTransit;

namespace eda.api.Business.BillService
{
    public class BillServiceBusiness : IConsumer<VisitRegisteredSendModel>
    {
        public async Task Consume(ConsumeContext<VisitRegisteredSendModel> context)
        {
            if (context.Message.PriceAfterTax != null)
            {
                new Exception("there should be no price at this moment");
            }

            var priceAfterTax = CalculateBill(context.Message.VisitEntity.VisitPrice);

            await context.Send<VisitRegisteredSendModel>(new Uri("queue:send_visit_registered_return_email"),
                new VisitRegisteredSendModel
                {
                    VisitEntity = context.Message.VisitEntity,
                    PriceAfterTax = priceAfterTax
                });
        }

        private double CalculateBill(int visitPrice)
        {
            var taxPercentage = 1.23;
            return visitPrice * taxPercentage;
        }
    }
}
