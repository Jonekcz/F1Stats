using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace F1_Stats.Models
{
    [Table("Kraj")]
    public partial class Country
    {
        public Country()
        {
            Drivers = new HashSet<Driver>();
            Cities = new HashSet<City>();
            Teams = new HashSet<Team>();
        }

        [Column("IdKraju")]
        public int CountryId { get; set; }
        [Column("Nazwa")]
        public string Name { get; set; }
        [Column("IdKontynentu")]
        public int ContinentId { get; set; }

        public virtual Continent ContinentIdNavigation { get; set; }
        public virtual ICollection<Driver> Drivers { get; set; }
        public virtual ICollection<City> Cities { get; set; }
        public virtual ICollection<Team> Teams { get; set; }
    }
}
