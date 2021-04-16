using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
#nullable disable

namespace F1_Stats.Models
{
    [Table("Miasto")]
    public partial class City
    {
        public City()
        {
            Circuits = new HashSet<Circuit>();
        }

        [Column("IdMiasta")]
        public int CityId { get; set; }
        [Column("Nazwa")]
        public string Name { get; set; }
        [Column("IdKraju")]
        public int CountryId { get; set; }

        public virtual Country CountryIdNavigation { get; set; }
        public virtual ICollection<Circuit> Circuits { get; set; }
    }
}
