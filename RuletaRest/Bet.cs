using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RuletaRest
{
    public class Bet
    {
        public string Key { get; set; }
        public int Value { get; set; }
        public bool Result { get; set; }
        public string RouletteId { get; set; }
    }
}
