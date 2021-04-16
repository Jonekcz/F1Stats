using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace F1_Stats.Models
{
    [Table("Sezon")]
    public partial class Season
    {
        public Season()
        {
            Events = new HashSet<Event>();
        }

        [Column("IdSezonu")]
        public int SeasonId { get; set; }
        [Column("Rok")]
        public short? Year { get; set; }

        public virtual ICollection<Event> Events { get; set; }
    }
}
