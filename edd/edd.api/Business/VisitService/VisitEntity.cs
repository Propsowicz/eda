using csStorage.Base.csEntityBaseModel;
using csStorage.Shared;

namespace edd.api.Business.VisitService
{
    public class VisitEntity : csEntityBaseModel<VisitEntity>
    {
        [csKey]
        public Guid Id { get; set; }

        public DateTime VisitDate { get; set; }

        public string PatientName { get; set; }

        public string PatientEmail { get; set; }

        public string DoctorName { get; set; }

        public int VisitPrice { get; set; }

        public string HospitalName { get; set; }
    }
}
