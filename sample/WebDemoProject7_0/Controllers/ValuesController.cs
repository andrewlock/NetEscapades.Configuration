using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace WebDemoProject2_0.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        private readonly IConfiguration _config;

        public ValuesController(IConfiguration config)
        {
            _config = config;
        }

        // GET api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
             return _config.AsEnumerable()
             .OrderBy(kvp => kvp.Key)
             .Select(kvp => kvp.Key + " - " + kvp.Value);
        }
    }
}
