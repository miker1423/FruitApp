using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using FruitAppAPI.Models;

namespace FruitAppAPI.Services.Interfaces
{
    public interface IProviderService
    {
        Task<List<Provider>> GetProviders();
        Task<Provider> GetProvider(Guid id);
        Task<Guid> CreateProvider(Provider provider);
        Task UpdateProvider(Provider provider);
    }
}
