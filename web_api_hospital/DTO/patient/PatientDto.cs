namespace web_api_hospital.DTO.patient
{
    public class PatientDto
    {
        public int Id { get; set; }
        public string Surname { get; set; }
        public string Firstname { get; set; }
        public string? Patronymic { get; set; }
        public string Address { get; set; }
        public DateTime Birthday { get; set; }
        public string Gender { get; set; }
        public int PlotId { get; set; }
    }
}
