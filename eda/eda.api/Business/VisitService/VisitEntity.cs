using csStorage.Base.csEntityBaseModel;
using csStorage.Shared;

namespace eda.api.Business.VisitService
{
    public class VisitEntity : csEntityBaseModel<VisitEntity>
    {
        public VisitEntity(Guid Id, DateTime VisitDate, string PatientName, 
            string PatientEmail, string DoctorName, int VisitPrice, string HospitalName)
        {
            this.Id = Id;
            this.VisitDate = VisitDate;
            this.PatientName = PatientName;
            this.PatientEmail = PatientEmail;
            this.DoctorName = DoctorName;
            this.VisitPrice = VisitPrice;
            this.HospitalName = HospitalName;
        }

        [csKey]
        public Guid Id { get; private set; }

        public DateTime VisitDate { get; private set; }

        public string PatientName { get; private set; }

        public string PatientEmail { get; private set; }

        public string DoctorName { get; private set; }

        public int VisitPrice { get; private set; }

        public string HospitalName { get; private set; }



        public static VisitEntity Create(DateTime visitDate, string patientName, 
            string patientEmail, string doctorName, int visitPrice, string hospitalName)
        {
            return new VisitEntity(
                Guid.NewGuid(),
                visitDate,
                patientName,
                patientEmail,
                doctorName,
                visitPrice,
                hospitalName
            );
        }
    }
}
