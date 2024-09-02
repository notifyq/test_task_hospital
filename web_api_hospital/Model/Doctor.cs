using System;
using System.Collections.Generic;

namespace web_api_hospital.Model;

public partial class Doctor
{
    public int Id { get; set; }

    public string Surname { get; set; } = null!;

    public string Firstname { get; set; } = null!;

    public string? Patronymic { get; set; }

    public int? PlotId { get; set; }

    public int? OfficeId { get; set; }

    public int SpecializationId { get; set; }

    public virtual Office? Office { get; set; }

    public virtual Plot? Plot { get; set; }

    public virtual Specialization Specialization { get; set; } = null!;
}
