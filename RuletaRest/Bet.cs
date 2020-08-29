using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RuletaRest
{
    public class Bet
    {
        public string UserId { get; set; }
        public string Value { get; set; }
        public int Amount { get; set; }
        public bool? Win { get; set; }
    }
}
