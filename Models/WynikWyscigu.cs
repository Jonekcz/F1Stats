using System;
using System.Collections.Generic;

#nullable disable

namespace F1_Stats.Models
{
    public partial class WynikWyscigu
    {
        public int IdWyscigu { get; set; }
        public int IdKierowcy { get; set; }
        public int? IdZespolu { get; set; }
        public byte? Punkty { get; set; }
        public byte? Pozycja { get; set; }
        public TimeSpan? Czas { get; set; }
        public int? IdRodzajuWyniku { get; set; }
        public TimeSpan? NajlepszyCzasOkrazenia { get; set; }

        public virtual Kierowca IdKierowcyNavigation { get; set; }
        public virtual RodzajWyniku IdRodzajuWynikuNavigation { get; set; }
        public virtual Wydarzenie IdWysciguNavigation { get; set; }
        public virtual Zespol IdZespoluNavigation { get; set; }
    }
}
