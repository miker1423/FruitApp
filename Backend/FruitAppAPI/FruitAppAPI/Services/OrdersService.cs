using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Neo4jClient;

using FruitAppAPI.Services.Interfaces;
using FruitAppAPI.NeoModels;

namespace FruitAppAPI.Services
{
    public class OrdersService : IOrdersService
    {
        private readonly IGraphClient _graphClient;
        public OrdersService(IGraphClient graphClient)
        {
            _graphClient = graphClient;
        }

        public IEnumerable<NeoProvider> FindProviders(string fruitName, List<string> certificates)
        {
            var query = _graphClient.Cypher
                .Match("(cert:NeoCertificate)-[:HAS]-(provider:NeoProvider)-[:CAN_SELL]-(fruit:NeoFruit)")
                .Where((NeoFruit fruit) => fruit.Name == fruitName);

            if(certificates != null || certificates?.Count != 0)
            {
                query.AndWhere("cert.Name IN {certificates}")
                .WithParam("certificates", certificates);
            }

            return query.Return(provider => provider.As<NeoProvider>())
                .Results
                .Distinct(new NeoProviderComparer());
        }
    }
}
