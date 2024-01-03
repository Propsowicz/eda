using MassTransit;

namespace eda.api.Messages.Commands
{
    public class CalculateVisitPrice : CorrelatedBy<Guid>
    {
        public Guid CorrelationId { get; set; }

        public Guid VisitId { get; set; }
    }
}
