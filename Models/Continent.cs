using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace F1_Stats.Models
{
    [Table("Kontynent")]
    public partial class Continent
    {
        public Continent()
        {
            Countries = new HashSet<Country>();
        }

        [Column("IdKontynentu")]
        public int IdKontynentu { get; set; }
        [Column("Nazwa")]
        public string Name { get; set; }

        public virtual ICollection<Country> Countries { get; set; }
    }
}
