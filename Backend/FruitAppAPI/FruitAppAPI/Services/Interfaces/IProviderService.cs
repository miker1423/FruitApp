using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using FruitAppAPI.Models;
using FruitAppAPI.ViewModels;

namespace FruitAppAPI.Services.Interfaces
{
    public interface IProviderService
    {
        Task<List<Provider>> GetProviders();
        Task<Provider> GetProvider(Guid id);
        Task<Guid> CreateProvider(ProviderVM provider);
        Task UpdateProvider(ProviderVM provider);
        Task Delete(Guid id);
        Task<IEnumerable<Provider>> FindProviders(string fruitName, List<string> certificates);
    }
}
