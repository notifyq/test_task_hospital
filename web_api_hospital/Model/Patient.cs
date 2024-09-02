using System;
using System.Collections.Generic;

namespace web_api_hospital.Model;

public partial class Patient
{
    public int Id { get; set; }

    public string Surname { get; set; } = null!;

    public string Firstname { get; set; } = null!;

    public string? Patronymic { get; set; }

    public string Address { get; set; } = null!;

    public DateTime Birthday { get; set; }

    public string Gender { get; set; } = null!;

    public int PlotId { get; set; }

    public virtual Plot Plot { get; set; } = null!;
}
