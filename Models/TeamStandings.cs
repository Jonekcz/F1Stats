using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace F1_Stats.Models
{
    [Keyless]
    [Table("Klasyfikacja konstruktorów")]
    public class TeamStandings
    {
        [Column("Nazwa")]
        public string Name { get; set; }
        [Column("Punkty")]
        public int Points { get; set; }
    }
}
