using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Models;
using StatusControl.Services;

namespace AccountingControl.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class StatusConsumptionController : Controller
    {
        private readonly ILogger<StatusConsumptionController> Logger;
        private readonly IStatusConsumptionService StatusConsumptionService;

        public StatusConsumptionController(ILogger<StatusConsumptionController> logger,
            IStatusConsumptionService statusConsumptionService)
        {
            Logger = logger;
            StatusConsumptionService = statusConsumptionService;
        }

        [HttpGet]
        public async Task<List<StatusConsumption>> GetStatusConsumption()
        {
            var statusConsumption = await StatusConsumptionService.GetStatusConsumptions();
            return statusConsumption;
        }
    }
}