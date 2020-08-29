using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Data;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using ServiceStack;
using ServiceStack.Redis;
using ServiceStack.Redis.Generic;
using StackExchange.Redis;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RuletaRest.Controllers
{
    [Microsoft.AspNetCore.Mvc.Route("api/[controller]")]
    [ApiController]
    public class RouletteController : ControllerBase
    {
        private readonly IDatabase _database;        
        private IRedisTypedClient <Roulette> client;
        private IRedisList roulettes;

        public RouletteController() 
        {
            using (RedisClient client = new RedisClient("localhost", 6379))
            {
                this.client = client.As<Roulette>();
                //this.client.FlushDb();
            }
          
        }
      
        // GET api/roulette
        [HttpGet]
        public IEnumerable<Roulette> Get()
        {
            List<Roulette> response= new List<Roulette>();
            var keys  = client.GetAllKeys();
            return client.GetValues(keys);
        }
        // GET api/roulette/5
        [HttpGet("{id}")]
        public Roulette Get(string id)
        {
            return client.GetValue(id);
        }



        // POST api/roulette
        [HttpPost]
        public string NewRoulette([FromBody] Roulette json)
        {
            var roulette = new Roulette()
            {
                Id = Guid.NewGuid().ToString(),
                State = "Open",
                Name = json.Name,
                Bets = new List<string>()
            };
           client.SetValue(roulette.Id,roulette);

            return roulette.Id;
        }

        // PUT api/roulette/open/5
        [HttpPut("open/{id}")]
        public string OpenRoulette(string id)
        {
            var roulette = client.GetValue(id);
            roulette.State = "Open";
            client.SetValue(roulette.Id, roulette);

            return "Roulette " + roulette.Name + " Open";
        }
        // PUT api/roulette/close/5
        [HttpPut("close/{id}")]
        public string CloseRoulette(string id)
        {
            var roulette = client.GetValue(id);
            roulette.State = "Closed";
            client.SetValue(roulette.Id, roulette);

            return "Roulette " + roulette.Name + " Closed";
        }

        // DELETE api/roulette/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
