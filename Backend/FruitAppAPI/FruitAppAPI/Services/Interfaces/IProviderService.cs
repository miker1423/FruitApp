﻿using System;
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
        Task UpdateProvider(Provider provider);
        Task Delete(Guid id);
    }
}
