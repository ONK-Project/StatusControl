using Models;
using MongoDB.Driver;
using StatusControl.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StatusControl.Services
{
    public class StatusConsumptionService : IStatusConsumptionService
    {
        private readonly IMongoCollection<StatusConsumption> _statusConsumptions;
        public StatusConsumptionService(IStatusControlDBSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _statusConsumptions = database.GetCollection<StatusConsumption>(settings.StatusConsumptionCollectionName);
        }
        public async Task<List<StatusConsumption>> GetStatusConsumptions()
        {
            var filter = new ExpressionFilterDefinition<StatusConsumption>(x => true);
            var sC = await _statusConsumptions.FindAsync(filter);
            return sC.ToList();
        }

        public Task PostStatusConsumption(StatusConsumption statusConsumption)
        {
            return _statusConsumptions.InsertOneAsync(statusConsumption);
        }
    }
}
