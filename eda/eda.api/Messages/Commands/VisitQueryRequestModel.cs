using MassTransit;

namespace eda.api.Messages.Commands
{
    public class VisitQueryRequestModel : CorrelatedBy<Guid>
    {
        public Guid CorrelationId { get; set; }

        public Guid Id { get; set; }
    }
}
