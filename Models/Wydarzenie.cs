using System;
using System.Collections.Generic;

#nullable disable

namespace F1_Stats.Models
{
    public partial class Wydarzenie
    {
        public Wydarzenie()
        {
            CzasOkrazenia = new HashSet<CzasOkrazenium>();
            Pitstops = new HashSet<Pitstop>();
            WynikKwalifikacjis = new HashSet<WynikKwalifikacji>();
            WynikWyscigus = new HashSet<WynikWyscigu>();
        }

        public int IdTerminarza { get; set; }
        public int IdToru { get; set; }
        public int IdSezonu { get; set; }
        public DateTime? DataCzas { get; set; }
        public string Pogoda { get; set; }
        public bool? CzerwonaFlaga { get; set; }

        public virtual Sezon IdSezonuNavigation { get; set; }
        public virtual Tor IdToruNavigation { get; set; }
        public virtual ICollection<CzasOkrazenium> CzasOkrazenia { get; set; }
        public virtual ICollection<Pitstop> Pitstops { get; set; }
        public virtual ICollection<WynikKwalifikacji> WynikKwalifikacjis { get; set; }
        public virtual ICollection<WynikWyscigu> WynikWyscigus { get; set; }
    }
}
