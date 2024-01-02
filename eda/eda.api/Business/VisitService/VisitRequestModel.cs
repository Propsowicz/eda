namespace eda.api.Business.VisitService
{
    public class VisitRequestModel
    {
        public DateTime VisitDate { get; set; }

        public string PatientName { get; set; }

        public string PatientEmail { get; set; }

        public string DoctorName { get; set; }

        public int VisitPrice { get; set; }

        public string HospitalName { get; set; }
    }
}
