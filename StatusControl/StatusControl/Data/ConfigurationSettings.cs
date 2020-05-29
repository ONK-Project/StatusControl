using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StatusControl.Data
{
    public interface IStatusControlDBSettings
    {
        string StatusConsumptionCollectionName { get; set; }
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }
    }

    public class StatusControlDBSettings : IStatusControlDBSettings
    {
        public string StatusConsumptionCollectionName { get; set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }
}
