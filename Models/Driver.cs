using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace F1_Stats.Models
{
    [Table("Kierowca")]
    public partial class Driver
    {
        public Driver()
        {
            LapTimes = new HashSet<LapTime>();
            Pitstops = new HashSet<Pitstop>();
            QualifyingResults = new HashSet<QualifyingResult>();
            Results = new HashSet<Result>();
        }
        [Column("IdKierowcy")]
        public int DriverId { get; set; }
        [Column("Imie"), Display(Name = "Imię")]
        public string Name { get; set; }
        [Column("Nazwisko"), Display(Name = "Nazwisko")]
        public string Lastname { get; set; }
        [Column("IdKraju")]
        public int? CountryId { get; set; }
        [DataType(DataType.Date), Display(Name = "Data urodzenia"), Column("DataUrodzenia")]
        public DateTime? DateOfBirth { get; set; }
        [Display(Name = "Kraj")]
        public virtual Country CountryIdNavigation { get; set; }
        public virtual ICollection<LapTime> LapTimes { get; set; }
        public virtual ICollection<Pitstop> Pitstops { get; set; }
        public virtual ICollection<QualifyingResult> QualifyingResults { get; set; }
        public virtual ICollection<Result> Results { get; set; }
    }
}
