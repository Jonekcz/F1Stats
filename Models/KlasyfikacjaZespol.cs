using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace F1_Stats.Models
{
    [Keyless]
    public class KlasyfikacjaZespol
    {
        public string Nazwa { get; set; }
        public int Punkty { get; set; }
    }
}
