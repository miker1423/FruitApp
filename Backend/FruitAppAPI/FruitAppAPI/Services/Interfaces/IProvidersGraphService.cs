using FruitAppAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FruitAppAPI.Services.Interfaces
{
    public interface IProvidersGraphService
    {
        Task CreateProvider(Provider provider);
    }
}
