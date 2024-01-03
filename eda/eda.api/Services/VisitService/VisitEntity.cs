using csStorage.Base.csEntityBaseModel;
using csStorage.Shared;

namespace eda.api.Services.VisitService
{
    public class VisitEntity : csEntityBaseModel<VisitEntity>
    {
        [csKey]
        public Guid Id { get; set; }

        public DateTime VisitDate { get; set; }

        public string PatientName { get; set; } = default!;

        public string PatientEmail { get; set; } = default!;

        public string DoctorName { get; set; } = default!;

        public string HospitalName { get; set; } = default!;

        public bool IsCancelled { get; set; }
    }
}
