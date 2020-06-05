using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace StatusControl.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StatusControlController : ControllerBase
    {
       

        private readonly ILogger<StatusControlController> _logger;

        public StatusControlController(ILogger<StatusControlController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            
        }
    }
}
