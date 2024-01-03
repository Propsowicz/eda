using MassTransit;

namespace eda.api.Messages.Events
{
    public class NewVisitRegistered : CorrelatedBy<Guid>
    {
        public Guid CorrelationId { get; set; }
        
        public DateTime VisitDate { get; set; }

        public string PatientName { get; set; } = default!;
        
        public string DoctorName { get; set; } = default!;

        public string HospitalName { get; set; } = default!;
    }
}
