using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Neo4jClient;

using FruitAppAPI.Models;
using FruitAppAPI.NeoModels;
using FruitAppAPI.ViewModels;
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

        public async Task CreateProvider(ProviderVM provider)
        {
            await _graphClient.Cypher
                              .Match("(fruit:NeoFruit)")
                              .Match("(cert:NeoCertificate)")
                              .Where((NeoFruit fruit) => provider.Fruits.Contains(fruit.Name))
                              .Where((NeoCertificate certificate )=> provider.Certificates.Contains(certificate.Name))
                              .Create("fruit<-[:CAN_SELL]-(provider:NeoProvider {newProvider})-[:HAS-CERTIFICATE]->certificate")
                              .WithParam("newProvider", new NeoProvider { Id = provider.Id.ToString() })
                              .ExecuteWithoutResultsAsync();
        }
    }
}
