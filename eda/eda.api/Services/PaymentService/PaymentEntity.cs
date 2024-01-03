using csStorage.Base.csEntityBaseModel;
using csStorage.Shared;

namespace eda.api.Services.PaymentService
{
    public class PaymentEntity : csEntityBaseModel<PaymentEntity>
    {
        [csKey]
        public Guid Id { get; set; }

        public Guid VisitId { get; set; }

        public double VisitPrice { get; set; }

        public DateTime DateOfPayment { get; set; }

        public bool IsPayed { get; set; }
    }
}
