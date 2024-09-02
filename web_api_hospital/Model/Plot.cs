using System;
using System.Collections.Generic;

namespace web_api_hospital.Model;

public partial class Plot
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public virtual ICollection<Doctor> Doctors { get; set; } = new List<Doctor>();

    public virtual ICollection<Patient> Patients { get; set; } = new List<Patient>();
}
