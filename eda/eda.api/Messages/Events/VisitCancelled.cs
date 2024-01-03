using MassTransit;

namespace eda.api.Messages.Events
{
    public class VisitCancelled : CorrelatedBy<Guid>
    {
        public Guid CorrelationId { get; set; }

        public CancellationReason CancellationReason { get; set; }

        public DateTime VisitDate { get; set; }

        public string PatientName { get; set; } = default!;

        public string DoctorName { get; set; } = default!;

        public string HospitalName { get; set; } = default!;
    }

    public enum CancellationReason
    {
        PaymentExpiration
    }
}
