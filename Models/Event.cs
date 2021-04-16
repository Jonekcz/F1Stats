using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace F1_Stats.Models
{
    [Table("Wydarzenie")]
    public partial class Event
    {
        public Event()
        {
            LapTimes = new HashSet<LapTime>();
            Pitstops = new HashSet<Pitstop>();
            QualifyingResults = new HashSet<QualifyingResult>();
            Results = new HashSet<Result>();
        }

        [Column("IdTerminarza")]
        public int EventId { get; set; }
        [Column("IdToru")]
        public int CircuitId { get; set; }
        [Column("IdSezonu")]
        public int SeasonId { get; set; }
        [Column("DataCzas")]
        public DateTime? DateTime { get; set; }
        [Column("Pogoda")]
        public string Weather { get; set; }
        [Column("CzerwonaFlaga")]
        public bool? RedFlag { get; set; }

        public virtual Season SeasonIdNavigation { get; set; }
        public virtual Circuit CircuitIdNavigation { get; set; }
        public virtual ICollection<LapTime> LapTimes { get; set; }
        public virtual ICollection<Pitstop> Pitstops { get; set; }
        public virtual ICollection<QualifyingResult> QualifyingResults { get; set; }
        public virtual ICollection<Result> Results { get; set; }
    }
}
