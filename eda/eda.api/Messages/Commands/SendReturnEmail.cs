using MassTransit;

namespace eda.api.Messages.Commands
{
    public class SendReturnEmail : CorrelatedBy<Guid>
    {
        public Guid CorrelationId { get; set; }

        public Guid PaymentId { get; set; }

        public string PatientEmail { get; set; } = default!;

        public DateTime VisitDate { get; set; }

        public string HospitalName { get; set; } = default!;

        public DateTime DayOfPayment { get; set; }
        
        public double VisitPrice { get; set; }
    }
}
