using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace F1_Stats.Models
{
    [Keyless]
    [Table("Klasyfikacja kierowców")]
    public class DriverStandings
    {
        [Column("Imie")]
        public string Name { get; set; }
        [Column("Nazwisko")]
        public string Lastname { get; set; }
        [Column("Punkty")]
        public int Points { get; set; }
    }
}
