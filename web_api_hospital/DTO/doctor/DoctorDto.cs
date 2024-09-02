namespace web_api_hospital.DTO.doctor
{
    public class DoctorDto
    {
        public int Id { get; set; }
        public string Surname { get; set; }
        public string Firstname { get; set; }
        public string? Patronymic { get; set; }
        public int? PlotId { get; set; }
        public int? OfficeId { get; set; }
        public int SpecializationId { get; set; }
    }
}
