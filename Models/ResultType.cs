using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace F1_Stats.Models
{
    [Table("RodzajWyniku")]
    public partial class ResultType
    {
        public ResultType()
        {
            Results = new HashSet<Result>();
        }

        [Column("IdRodzajuWyniku")]
        public int ResultTypeId { get; set; }
        [Column("Nazwa")]
        public string Name { get; set; }

        public virtual ICollection<Result> Results { get; set; }
    }
}
