using eda.api.Business.VisitService;
using MassTransit;

namespace eda.api.EventModels
{
    public class VisitRegisteredSendModel : CorrelatedBy<Guid>
    {
        public Guid CorrelationId { get; set; }
        public VisitEntity VisitEntity { get; set; }
        public double PriceAfterTax { get; set; }
    }
}
