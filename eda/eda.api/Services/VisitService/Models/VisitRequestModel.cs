namespace eda.api.Services.VisitService.Models
{
    public class VisitRequestModel
    {
        public DateTime VisitDate { get; set; }

        public string PatientName { get; set; } = default!;

        public string PatientEmail { get; set; } = default!;

        public string DoctorName { get; set; } = default!;
        
        public string HospitalName { get; set; } = default!;
    }
}
