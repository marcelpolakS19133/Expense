using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Expense.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<Account> Get()
        {
            var client = new MongoClient("mongodb+srv://TestUser:DPZEUE0mjq0dIXZ7@expensecluster.sbkbj.mongodb.net/ExpenseDatabase?retryWrites=true&w=majority");
            var database = client.GetDatabase("ExpenseDatabase");
            var collection = database.GetCollection<Account>("Konta");

            return collection.Find(s => s.Name == "Konto Kuby").ToList();
        }
    }
}
