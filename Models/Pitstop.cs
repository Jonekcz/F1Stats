﻿using System;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace F1_Stats.Models
{
    public partial class Pitstop
    {
        [Column("IdPitstopu")]
        public int PitstopId { get; set; }
        [Column("IdWyscigu")]
        public int RaceId { get; set; }
        [Column("IdKierowcy")]
        public int DriverId { get; set; }
        [Column("Okrazenie")]
        public byte? Lap { get; set; }
        [Column("CzasTrwania")]
        public TimeSpan? Duration { get; set; }

        public virtual Driver DriverIdNavigation { get; set; }
        public virtual Event RaceIdNavigation { get; set; }
    }
}
