using MassTransit;

namespace eda.api.Messages.Commands
{
    public class CheckIfPaymentExecuted : CorrelatedBy<Guid>
    {
        public Guid CorrelationId { get; set; }

        public Guid PaymentId { get; set; }
    }
}
