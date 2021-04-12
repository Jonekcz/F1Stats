using System;
using System.Collections.Generic;

#nullable disable

namespace F1_Stats.Models
{
    public partial class Sezon
    {
        public Sezon()
        {
            Wydarzenies = new HashSet<Wydarzenie>();
        }

        public int IdSezonu { get; set; }
        public short? Rok { get; set; }

        public virtual ICollection<Wydarzenie> Wydarzenies { get; set; }
    }
}
