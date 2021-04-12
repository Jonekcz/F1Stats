using System;
using System.Collections.Generic;

#nullable disable

namespace F1_Stats.Models
{
    public partial class RodzajWyniku
    {
        public RodzajWyniku()
        {
            WynikWyscigus = new HashSet<WynikWyscigu>();
        }

        public int IdRodzajuWyniku { get; set; }
        public string Nazwa { get; set; }

        public virtual ICollection<WynikWyscigu> WynikWyscigus { get; set; }
    }
}
