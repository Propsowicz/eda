namespace eda.api.Services.PaymentService.Models
{
    public class PaymentQueryResponseModel
    {
        public Guid Id { get; set; }

        public double VisitPrice { get; set; }

        public DateTime DateOfPayment { get; set; }

        public bool IsPayed { get; set; }
    }
}
