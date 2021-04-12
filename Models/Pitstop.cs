using System;
using System.Collections.Generic;

#nullable disable

namespace F1_Stats.Models
{
    public partial class Pitstop
    {
        public int IdPitstopu { get; set; }
        public int IdWyscigu { get; set; }
        public int IdKierowcy { get; set; }
        public byte? Okrazenie { get; set; }
        public TimeSpan? CzasTrwania { get; set; }

        public virtual Kierowca IdKierowcyNavigation { get; set; }
        public virtual Wydarzenie IdWysciguNavigation { get; set; }
    }
}
