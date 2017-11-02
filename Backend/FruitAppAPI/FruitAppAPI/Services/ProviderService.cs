﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Neo4jClient;

using FruitAppAPI.DBContexts;
using FruitAppAPI.Models;
using FruitAppAPI.Services.Interfaces;
using FruitAppAPI.Extensions;
using FruitAppAPI.NeoModels;
using FruitAppAPI.ViewModels;

namespace FruitAppAPI.Services
{
    public class ProviderService : IProviderService
    {
        private readonly IProvidersGraphService _providersGraphService;
        private readonly AppDBContext _DbContext;

        public ProviderService(
            IProvidersGraphService providersGraphService,
            AppDBContext dBContext)
        {
            _providersGraphService = providersGraphService;
            _DbContext = dBContext;
        }

        public async Task<Guid> CreateProvider(ProviderVM provider)
        {
            provider.Id = Guid.NewGuid();

            await _DbContext.Providers.AddAsync(provider.ToEntity());
            var saveSql = _DbContext.SaveChangesAsync();
            var createNode = _providersGraphService.CreateProvider(provider);

            await saveSql;
            await createNode;

            return provider.Id;
        }

        public async Task Delete(Guid id)
        {
            var query = _DbContext.Providers.Where(provider => provider.Id == id).First();
            _DbContext.Remove(query);
            await _providersGraphService.DeleteProvider(id);
            await _DbContext.SaveChangesAsync();
        }

        public Task<Provider> GetProvider(Guid id)
        {
            var query = _DbContext.Providers.Where(provider => provider.Id == id);
            if(query.Count() == 1)
            {
                return Task.FromResult(query.First());
            }

            throw new Exception("Multiple providers found");
        }

        public Task<List<Provider>> GetProviders() => Task.FromResult(_DbContext.Providers.ToList());

        public async Task UpdateProvider(ProviderVM provider)
        {
            _DbContext.Providers.Update(provider.ToEntity());
            await _DbContext.SaveChangesAsync();

            await _providersGraphService.UpdateProvider(provider);
        }
    }
}
