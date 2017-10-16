using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Neo4jClient;
using Neo4jClient.Transactions;

using FruitAppAPI.NeoModels;
using FruitAppAPI.Services.Interfaces;

namespace FruitAppAPI.Services
{
    public class CertificatesService : ICertificatesService
    {
        private readonly IGraphClient _graphClient;
        public CertificatesService(IGraphClient graphClient)
        {
            _graphClient = graphClient;
        }

        public Task CreateCertificates(List<string> certificates)
        {
            var createTasks = new List<Task>();

            foreach (var certificate in certificates)
            {
                createTasks.Add(CreateIfNotExists(certificate));
            }

            return Task.WhenAll(createTasks);
        }

        public Task<IEnumerable<string>> GetCertificates()
        {
            return Task.FromResult(_graphClient.Cypher
                .Match("(cert:NeoCertificate)")
                .Return(cert => cert.As<NeoCertificate>())
                .Results
                .Select(cert => cert.Name)
                .Distinct());
        }


        public Task FindAndRelate(string nodeId, string certName)
        {
            return _graphClient.Cypher
                .Match("(cert:NeoCertificate)", "(provider:NeoProvider)")
                .Where((NeoCertificate cert) => cert.Name == certName)
                .AndWhere((NeoProvider provider) => provider.Id == nodeId)
                .Create("(provider)-[:HAS]->(cert)")
                .ExecuteWithoutResultsAsync();
        }

        private Task CreateIfNotExists(string fruitName)
        {
            var newCertificate = new NeoCertificate
            {
                Name = fruitName
            };

            return _graphClient.Cypher
                .Merge("(certificate:NeoCertificate { Name: {name}})")
                .OnCreate()
                .Set("certificate = {newCertificate}")
                .WithParams(new
                {
                    name = newCertificate.Name,
                    newCertificate = newCertificate
                })
                .ExecuteWithoutResultsAsync();
        }
    }
}
