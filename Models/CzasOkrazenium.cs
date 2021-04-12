using System;
using System.Collections.Generic;

#nullable disable

namespace F1_Stats.Models
{
    public partial class CzasOkrazenium
    {
        public int IdWyscigu { get; set; }
        public int IdKierowcy { get; set; }
        public byte Okrazenie { get; set; }
        public TimeSpan? Czas { get; set; }

        public virtual Kierowca IdKierowcyNavigation { get; set; }
        public virtual Wydarzenie IdWysciguNavigation { get; set; }
    }
}
