using System;
using System.Collections.Generic;

#nullable disable

namespace F1_Stats.Models
{
    public partial class Zespol
    {
        public Zespol()
        {
            WynikKwalifikacjis = new HashSet<WynikKwalifikacji>();
            WynikWyscigus = new HashSet<WynikWyscigu>();
        }

        public int IdZespolu { get; set; }
        public string Nazwa { get; set; }
        public string DostawcaSilnika { get; set; }
        public int? IdKraju { get; set; }

        public virtual Kraj IdKrajuNavigation { get; set; }
        public virtual ICollection<WynikKwalifikacji> WynikKwalifikacjis { get; set; }
        public virtual ICollection<WynikWyscigu> WynikWyscigus { get; set; }
    }
}
