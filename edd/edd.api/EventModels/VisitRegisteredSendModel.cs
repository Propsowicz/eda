using edd.api.Business.VisitService;

namespace edd.api.EventModels
{
    public class VisitRegisteredSendModel
    {
         public VisitEntity VisitEntity { get; set; }
         public double PriceAfterTax { get; set; }
    }
}
