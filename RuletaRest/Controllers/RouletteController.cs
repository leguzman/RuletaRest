using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using ServiceStack.Redis;
using ServiceStack.Redis.Generic;

namespace RuletaRest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RouletteController : ControllerBase
    {    
        private IRedisTypedClient <Roulette> client;
        List<string> BlackResults = new List<string> { "2", "4", "6", "8", "10", "11", "13", "15", "17", "20", "22", "24", "26", "28", "29", "31", "33", "35" };
        List<string> RedResults = new List<string> { "1", "3", "5", "7", "9", "12", "14", "16", "18", "19", "21", "23", "25", "27", "30", "32", "34", "36" };
        
        public RouletteController() 
        {
            using (RedisClient client = new RedisClient("localhost", 6379))
            {
                this.client = client.As<Roulette>();
                //this.client.FlushDb();
            }
        }

        [HttpGet]
        public IEnumerable<Roulette> Get()
        {
            var keys  = client.GetAllKeys();

            return client.GetValues(keys);
        }

        [HttpGet("{id}")]
        public Roulette Get(string id)
        {
            return client.GetValue(id);
        }

        [HttpPost]
        public string NewRoulette([FromBody] Roulette json)
        {
            var roulette = new Roulette()
            {
                Id = Guid.NewGuid().ToString(),
                State = "Open",
                Name = json.Name,
                Bets = new List<Bet>()
            };
           client.SetValue(roulette.Id,roulette);

            return roulette.Id;
        }

        [HttpPut("open/{id}")]
        public string OpenRoulette(string id)
        {
            var roulette = client.GetValue(id);
            if(roulette is null)
            {
                return "Operation Failed: roulette doesn't exist.";
            }
            roulette.State = "Open";
            roulette.Bets.Clear();
            client.SetValue(roulette.Id, roulette);

            return "Roulette " + roulette.Name + " Open";
        }

        [HttpPut("close/{id}")]
        public Roulette CloseRoulette(string id)
        {
            var roulette = client.GetValue(id);
            if (roulette is null)
            {
                return null;
            }
            roulette.State = "Closed";
            CloseBets(roulette);
            client.SetValue(roulette.Id, roulette);

            return roulette;
        }

        

        [HttpPost("{id}/bet")]
        public string SetBet(string id,[FromHeader]string userId, [FromBody] Bet bet)
        {
            if (!RedResults.Contains(bet.Value) && !BlackResults.Contains(bet.Value) && bet.Value!="Red" && bet.Value!="Black")
            {
                return "Invalid value for bet. Try again.";
            }
            if(bet.Amount<1 || bet.Amount > 10000)
            {
                return "Invalid amount for bet. Try again.";
            }
            var roulette = client.GetValue(id);
            if(roulette.State is "Open")
            {
                bet.UserId = userId;
                roulette.Bets.Add(bet);
                client.SetValue(roulette.Id, roulette);
            }
            else
            {
                return "Invalid Roulette Id. Roulette is not open.";
            }

            return "Bet has been set.";
        }

        private void CloseBets(Roulette roulette)
        {
            var rnd = new Random();
            var winNumber = rnd.Next(1, 37).ToString();
            foreach(Bet bet in roulette.Bets)
            {
                if (bet.Value is "Red" || bet.Value is "Black")
                {
                    if ((RedResults.Contains(winNumber) && bet.Value is "Red") || (BlackResults.Contains(winNumber) && bet.Value is "Black")) bet.Win = true;
                    else bet.Win = false;
                }
                else if (bet.Value == winNumber)
                {
                    bet.Win = true;
                }
                else
                {
                    bet.Win = false;
                }
            }

        }
    }
}
