using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebConfigurationProvider.Controllers
{

    [Route("api/[controller]")]
    public class AuthorizeConfigurationController : Controller
    {
        // GET api/AuthorizeConfiguration
        [HttpGet]
        public Dictionary<string, string> Get()
        {
            return new Dictionary<string, string>
            {
                { "remote:authorize_value1", "authorize1" },
                { "remote:authorize_value2", "authorize2" },
            };
        }
    }
}
