using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace F1_Stats.Models
{
    [Table("WynikWyscigu")]
    public partial class Result
    {
        [Column("IdWyscigu")]
        public int RaceId { get; set; }
        [Column("IdKierowcy")]
        public int DriverId { get; set; }
        [Column("IdZespolu")]
        public int? TeamId { get; set; }
        [Display(Name ="Points")]
        [Column("Punkty")]
        public byte? Points { get; set; }
        [Display(Name="Position")]
        [Column("Pozycja")]
        public byte? Position { get; set; }
        [Column("Czas")]
        public TimeSpan? Time { get; set; }
        /* used for there's no time, e.g + 1 lap*/
        [NotMapped]
        public string ResultType { get; set; }
        [Column("IdRodzajuWyniku")]
        public int? ResultTypeId { get; set; }
        [Column("NajlepszyCzasOkrazenia")]
        public TimeSpan? BestLapTime { get; set; }

        public virtual Driver DriverIdNavigation { get; set; }
        public virtual ResultType ResultTypeIdNavigation { get; set; }
        public virtual Event RaceIdNavigation { get; set; }
        public virtual Team TeamIdNavigation { get; set; }
    }
}
