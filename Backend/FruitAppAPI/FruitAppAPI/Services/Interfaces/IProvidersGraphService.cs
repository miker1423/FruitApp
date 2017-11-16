using FruitAppAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using FruitAppAPI.ViewModels;
using FruitAppAPI.NeoModels;

namespace FruitAppAPI.Services.Interfaces
{
    public interface IProvidersGraphService
    {
        Task CreateProvider(ProviderVM provider);
        Task UpdateProvider(ProviderVM provider);
        Task DeleteProvider(Guid id);
        IEnumerable<NeoProvider> FindProviders(string fruitName, List<string> certificates);
    }
}
