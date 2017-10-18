using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FruitAppAPI.NeoModels
{
    public class NeoProvider
    {
        public string Id { get; set; }
    }

    public class NeoProviderComparer : IEqualityComparer<NeoProvider>
    {
        public bool Equals(NeoProvider x, NeoProvider y) =>
            x.Id == y.Id;

        public int GetHashCode(NeoProvider obj) => obj.Id.GetHashCode();
    }
}
