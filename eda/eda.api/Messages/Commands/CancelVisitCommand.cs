using eda.api.Messages.Events;
using MassTransit;

namespace eda.api.Messages.Commands
{
    public class CancelVisitCommand : CorrelatedBy<Guid>
    {
        public Guid CorrelationId { get; set; }

        public Guid VisitId { get; set; }

        public CancellationReason CancellationReason { get; set; }
    }
}
