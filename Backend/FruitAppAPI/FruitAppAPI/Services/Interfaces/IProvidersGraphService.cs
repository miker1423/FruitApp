using FruitAppAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using FruitAppAPI.ViewModels;

namespace FruitAppAPI.Services.Interfaces
{
    public interface IProvidersGraphService
    {
        Task CreateProvider(ProviderVM provider);
        Task DeleteProvider(Guid id);
    }
}
