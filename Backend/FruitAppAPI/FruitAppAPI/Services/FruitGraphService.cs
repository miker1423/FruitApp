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
    public class FruitGraphService : IFruitGraphService
    {
        private readonly IGraphClient _graphClient;
        public FruitGraphService(IGraphClient graphClient)
        {
            _graphClient = graphClient;
        }

        public Task CreateFruits(List<string> fruits)
        {
            var createTasks = new List<Task>();

            foreach (var fruit in fruits)
            {
                createTasks.Add(CreateIfNotExists(fruit));
            }

            return Task.WhenAll(createTasks);
        }

        public Task<IEnumerable<string>> GetFruits()
        {
            return Task.FromResult(_graphClient.Cypher
                .Match("(fruit:NeoFruit)")
                .Return(fruit => fruit.As<NeoFruit>())
                .Results
                .Select(fruit => fruit.Name)
                .Distinct());
        }

        public Task FindAndRelate(string nodeId, string fruitName)
        {
            return _graphClient.Cypher
                .Match("(fruit:NeoFruit)", "(provider:NeoProvider)")
                .Where((NeoFruit fruit) => fruit.Name == fruitName)
                .AndWhere((NeoProvider provider) => provider.Id == nodeId)
                .Create("(provider)-[:CAN_SELL]->(fruit)")
                .ExecuteWithoutResultsAsync();
        }
        
        private Task CreateIfNotExists(string fruitName)
        {
            var newFruit = new NeoFruit
            {
                Name = fruitName
            };

            return _graphClient.Cypher
                .Merge("(fruit:NeoFruit { Name: {name}})")
                .OnCreate()
                .Set("fruit = {newFruit}")
                .WithParams(new
                {
                    name = newFruit.Name,
                    newFruit = newFruit
                })
                .ExecuteWithoutResultsAsync();
        }
    }
}
