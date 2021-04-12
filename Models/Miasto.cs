using System;
using System.Collections.Generic;

#nullable disable

namespace F1_Stats.Models
{
    public partial class Miasto
    {
        public Miasto()
        {
            Tors = new HashSet<Tor>();
        }

        public int IdMiasta { get; set; }
        public string Nazwa { get; set; }
        public int IdKraju { get; set; }

        public virtual Kraj IdKrajuNavigation { get; set; }
        public virtual ICollection<Tor> Tors { get; set; }
    }
}
