using System;
using System.Collections.Generic;

#nullable disable

namespace F1_Stats.Models
{
    public partial class Kraj
    {
        public Kraj()
        {
            Kierowcas = new HashSet<Kierowca>();
            Miastos = new HashSet<Miasto>();
            Zespols = new HashSet<Zespol>();
        }

        public int IdKraju { get; set; }
        public string Nazwa { get; set; }
        public int IdKontynentu { get; set; }

        public virtual Kontynent IdKontynentuNavigation { get; set; }
        public virtual ICollection<Kierowca> Kierowcas { get; set; }
        public virtual ICollection<Miasto> Miastos { get; set; }
        public virtual ICollection<Zespol> Zespols { get; set; }
    }
}
