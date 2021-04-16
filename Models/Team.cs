using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace F1_Stats.Models
{
    [Table("Zespol")]
    public partial class Team
    {
        public Team()
        {
            QualifyingResults = new HashSet<QualifyingResult>();
            Results = new HashSet<Result>();
        }

        [Column("IdZespolu")]
        public int TeamId { get; set; }
        [Column("Nazwa")]
        public string Name { get; set; }
        [Column("DostawcaSilnika")]
        public string EngineSupplier { get; set; }
        [Column("IdKraju")]
        public int? CountryId { get; set; }

        public virtual Country CountryIdNavigation { get; set; }
        public virtual ICollection<QualifyingResult> QualifyingResults { get; set; }
        public virtual ICollection<Result> Results { get; set; }
    }
}
