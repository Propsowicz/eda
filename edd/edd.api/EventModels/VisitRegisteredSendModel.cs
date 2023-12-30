using edd.api.Business.VisitService;
using MassTransit;

namespace edd.api.EventModels
{
    public class VisitRegisteredSendModel : CorrelatedBy<Guid>
    {
        public Guid CorrelationId { get; set; }
        public VisitEntity VisitEntity { get; set; }
        public double PriceAfterTax { get; set; }
    }
}
