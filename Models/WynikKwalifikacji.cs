using System;
using System.Collections.Generic;

#nullable disable

namespace F1_Stats.Models
{
    public partial class WynikKwalifikacji
    {
        public int IdKwalifikacji { get; set; }
        public int IdKierowcy { get; set; }
        public int? IdZespolu { get; set; }
        public byte? Pozycja { get; set; }
        public TimeSpan? CzasQ1 { get; set; }
        public TimeSpan? CzasQ2 { get; set; }
        public TimeSpan? CzasQ3 { get; set; }

        public virtual Kierowca IdKierowcyNavigation { get; set; }
        public virtual Wydarzenie IdKwalifikacjiNavigation { get; set; }
        public virtual Zespol IdZespoluNavigation { get; set; }
    }
}
