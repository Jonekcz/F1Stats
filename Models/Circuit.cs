using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace F1_Stats.Models
{
    [Table("Tor")]
    public partial class Circuit
    {
        public Circuit()
        {
            Events = new HashSet<Event>();
        }

        [Column("IdToru")]
        public int CircuitId { get; set; }
        [Column("Nazwa")]
        [Required]
        [MinLength(3)]
        public string Name { get; set; }
        [Column("IdMiasta")]
        public int CityId { get; set; }
        [Column("DlugoscGeo")]
        public decimal? Lng { get; set; }
        [Column("SzerokoscGeo")]
        public decimal? Lat { get; set; }

        public virtual City CityIdNavigation { get; set; }
        public virtual ICollection<Event> Events { get; set; }
    }
}
