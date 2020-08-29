using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RuletaRest
{
    public class Roulette
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string State { get; set; } = "Closed";
        public List<string> Bets { get; set; }
    }
}
