using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace WebConfigurationProvider.Controllers
{
    [Route("api/[controller]")]
    public class ConfigurationController : Controller
    {
        // GET api/values
        [HttpGet]
        public Dictionary<string, string> Get()
        {
            return new Dictionary<string, string>
            {
                { "remote:inner1:value1", "thevalue" },
                { "remote:inner1:value2", "2nd value" },
                { "remote:value1", "1" },
                { "remote:value2", "2" },
            };
        }
    }
}
