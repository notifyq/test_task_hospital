namespace web_api_hospital.DTO.doctor
{
    public class DoctorListDto
    {
        public int Id { get; set; }
        public string Surname { get; set; }
        public string Firstname { get; set; }
        public string? Patronymic { get; set; }
        public string PlotName { get; set; }
        public string OfficeName { get; set; }
        public string SpecializationName { get; set; }
    }
}
