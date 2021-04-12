using System;
using System.Collections.Generic;

#nullable disable

namespace F1_Stats.Models
{
    public partial class Tor
    {
        public Tor()
        {
            Wydarzenies = new HashSet<Wydarzenie>();
        }

        public int IdToru { get; set; }
        public string Nazwa { get; set; }
        public int IdMiasta { get; set; }
        public decimal? DlugoscGeo { get; set; }
        public decimal? SzerokoscGeo { get; set; }

        public virtual Miasto IdMiastaNavigation { get; set; }
        public virtual ICollection<Wydarzenie> Wydarzenies { get; set; }
    }
}
