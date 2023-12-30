using MassTransit;

namespace edd.api.EventModels
{
    public class VisitRegisteredPublishModel : CorrelatedBy<Guid>
    {
        public string Message { get; set; }

        public Guid CorrelationId { get; set; }
    }
}
