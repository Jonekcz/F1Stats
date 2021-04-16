using System;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace F1_Stats.Models
{
    [Table("CzasOkrazenia")]
    public partial class LapTime
    {
        [Column("IdWyscigu")]
        public int RaceId { get; set; }
        [Column("IdKierowcy")]
        public int DriverId { get; set; }
        [Column("Okrazenie")]
        public byte Lap { get; set; }
        [Column("Czas")]
        public TimeSpan? Time { get; set; }

        public virtual Driver DriverIdNavigation { get; set; }
        public virtual Event RaceIdNavigation { get; set; }
    }
}
