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
        private readonly IFruitGraphService _fruitGraphService;
        private readonly ICertificatesService _certificatesService;

        public ProviderGraphService(
            ICertificatesService certificatesService,
            IFruitGraphService fruitGraphService,
            IGraphClient graphClient)
        {
            _graphClient = graphClient;
            _fruitGraphService = fruitGraphService;
            _certificatesService = certificatesService;
        }

        public async Task CreateProvider(ProviderVM provider)
        {
            await _fruitGraphService.CreateFruits(provider.Fruits);
            await _certificatesService.CreateCertificates(provider.Certificates);

            var newProvider = new NeoProvider { Id = provider.Id.ToString() };

            await _graphClient.Cypher
                .Match("(fruit:NeoFruit)", "(cert:NeoCertificate)")
                .Where("fruit.Name IN {fruits}")
                .WithParam("fruits", provider.Fruits)
                .AndWhere("cert.Name IN {certificates}")
                .WithParam("certificates", provider.Certificates)
                .CreateUnique("(cert)<-[:HAS]-(provider:NeoProvider {newProvider})-[:CAN_SELL]->(fruit)")
                .WithParam("newProvider", provider)
                .ExecuteWithoutResultsAsync();
        }

        public async Task UpdateProvider(ProviderVM provider)
        {
            await _fruitGraphService.CreateFruits(provider.Fruits);
            await _certificatesService.CreateCertificates(provider.Certificates);

            await _graphClient.Cypher
                .Match("(fruit:NeoFruit)", "(cert:NeoCertificate)", "(provider:NeoProvider")
                .Where("fruit.Name IN {fruits}")
                .WithParam("fruits", provider.Fruits)
                .AndWhere("cert.Name IN {certificates}")
                .WithParam("certificates", provider.Certificates)
                .Where((NeoProvider neoProvider) => neoProvider.Id == provider.Id.ToString())
                .CreateUnique("(cert)<-[:HAS]-(provider:NeoProvider {newProvider})-[:CAN_SELL]->(fruit)")
                .WithParam("newProvider", provider)
                .ExecuteWithoutResultsAsync();
        }

        public Task DeleteProvider(Guid id) =>
            _graphClient.Cypher
                .OptionalMatch("(provider:NeoProvider)-[r]->()")
                .Where((NeoProvider provider) => provider.Id == id.ToString())
                .Delete("r, provider")
                .ExecuteWithoutResultsAsync();
    }
}
