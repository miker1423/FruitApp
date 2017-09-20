using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using FruitAppAPI.DBContexts;
using FruitAppAPI.Models;
using FruitAppAPI.Services.Interfaces;

namespace FruitAppAPI.Services
{
    public class ProviderService : IProviderService
    {
        private readonly AppDBContext _DbContext;
        public ProviderService(AppDBContext dBContext)
        {
            _DbContext = dBContext;
        }

        public async Task<Guid> CreateProvider(Provider provider)
        {
            provider.Id = Guid.NewGuid();
            await _DbContext.Providers.AddAsync(provider);
            await _DbContext.SaveChangesAsync();

            return provider.Id;
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

        public async Task UpdateProvider(Provider provider)
        {
            _DbContext.Providers.Update(provider);
            await _DbContext.SaveChangesAsync();
        }
    }
}
