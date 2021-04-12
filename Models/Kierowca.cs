using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace F1_Stats.Models
{
    public partial class Kierowca
    {
        public Kierowca()
        {
            CzasOkrazenia = new HashSet<CzasOkrazenium>();
            Pitstops = new HashSet<Pitstop>();
            WynikKwalifikacjis = new HashSet<WynikKwalifikacji>();
            WynikWyscigus = new HashSet<WynikWyscigu>();
        }

        public int IdKierowcy { get; set; }
        public string Imie { get; set; }
        public string Nazwisko { get; set; }
        public int? IdKraju { get; set; }
        [DataType(DataType.Date),Display(Name = "Data urodzenia")]
        public DateTime? DataUrodzenia { get; set; }
        [Display(Name = "Kraj")]
        public virtual Kraj IdKrajuNavigation { get; set; }
        public virtual ICollection<CzasOkrazenium> CzasOkrazenia { get; set; }
        public virtual ICollection<Pitstop> Pitstops { get; set; }
        public virtual ICollection<WynikKwalifikacji> WynikKwalifikacjis { get; set; }
        public virtual ICollection<WynikWyscigu> WynikWyscigus { get; set; }
    }
}
