using System;
using System.Collections.Generic;

#nullable disable

namespace F1_Stats.Models
{
    public partial class Kontynent
    {
        public Kontynent()
        {
            Krajs = new HashSet<Kraj>();
        }

        public int IdKontynentu { get; set; }
        public string Nazwa { get; set; }

        public virtual ICollection<Kraj> Krajs { get; set; }
    }
}
