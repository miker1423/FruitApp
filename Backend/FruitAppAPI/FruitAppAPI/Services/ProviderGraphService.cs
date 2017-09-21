using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Neo4jClient;

using FruitAppAPI.Models;
using FruitAppAPI.NeoModels;
using FruitAppAPI.Services.Interfaces;

namespace FruitAppAPI.Services
{
    public class ProviderGraphService : IProvidersGraphService
    {
        private readonly IGraphClient _graphClient;

        public ProviderGraphService(IGraphClient graphClient)
        {
            _graphClient = graphClient;
        }

        public async Task CreateProvider(Provider provider)
        {
            var fruitList = new List<string>();
            await _graphClient.Cypher
                              .Match("(fruit:NeoFruit)")
                              .Where((NeoFruit fruit) => fruitList.Contains(fruit.Name))
                              .Create("fruit<-[:CAN_SELL]-(provider:NeoProvider {newProvider})")
                              .WithParam("newProvider", new NeoProvider { Id = provider.Id.ToString() })
                              .ExecuteWithoutResultsAsync();
        }
    }
}
