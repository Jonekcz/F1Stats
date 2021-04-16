using System;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace F1_Stats.Models
{
    [Table("WynikKwalifikacji")]
    public partial class QualifyingResult
    {
        [Column("IdKwalifikacji")]
        public int QualifyingId { get; set; }
        [Column("IdKierowcy")]
        public int DriverId { get; set; }
        [Column("IdZespolu")]
        public int? TeamId { get; set; }
        [Column("Pozycja")]
        public byte? Position { get; set; }
        [Column("CzasQ1")]
        public TimeSpan? Q1Time { get; set; }
        [Column("CzasQ2")]
        public TimeSpan? Q2Time { get; set; }
        [Column("CzasQ3")]
        public TimeSpan? Q3Time { get; set; }

        public virtual Driver DriverIdNavigation { get; set; }
        public virtual Event QualifyingIdNavigation { get; set; }
        public virtual Team TeamIdNavigation { get; set; }
    }
}
