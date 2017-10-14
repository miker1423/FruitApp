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

            await CreateProvider(newProvider);
            var certList = new List<Task>();
            var fruitList = new List<Task>();

            var fruitTask = Task.Run(() =>
            {
                foreach (var fruit in provider.Fruits)
                {
                    fruitList.Add(_fruitGraphService.FindAndRelate(newProvider.Id, fruit));
                }
            });

            var certTask = Task.Run(() =>
            {
                foreach (var cert in provider.Certificates)
                {
                    fruitList.Add(_certificatesService.FindAndRelate(newProvider.Id, cert));
                }
            });

            await Task.WhenAll(fruitTask, certTask);
            await Task.WhenAll(certList);
            await Task.WhenAll(fruitList);
        }
        
        private Task CreateProvider(NeoProvider newProvider)
        {
            return _graphClient.Cypher
                .Create("(provider:NeoProvider {newProvider})")
                .WithParam("newProvider", newProvider)
                .ExecuteWithoutResultsAsync();
        }
    }
}
