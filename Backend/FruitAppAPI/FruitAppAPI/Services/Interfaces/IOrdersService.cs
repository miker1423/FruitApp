using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using FruitAppAPI.NeoModels;

namespace FruitAppAPI.Services.Interfaces
{
    public interface IOrdersService
    {
        IEnumerable<NeoProvider> FindProviders(string fruit, List<string> certificates);
    }
}
