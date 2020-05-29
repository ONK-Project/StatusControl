using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StatusControl.Services
{
    public interface IStatusConsumptionService
    {
        Task<List<StatusConsumption>> GetStatusConsumptions();
        Task PostStatusConsumption(StatusConsumption accountConsumption);
    }
}
