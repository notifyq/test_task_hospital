using System;
using System.Collections.Generic;

namespace web_api_hospital.Model;

    public partial class Office
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public virtual ICollection<Doctor> Doctors { get; set; } = new List<Doctor>();
    }
