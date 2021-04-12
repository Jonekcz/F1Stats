using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace F1_Stats.Models
{
    [Keyless]
    public class KlasyfikacjaKierowca
    {
        public string Imie { get; set; }
        public string Nazwisko { get; set; }
        public int Punkty { get; set; }
    }
}
