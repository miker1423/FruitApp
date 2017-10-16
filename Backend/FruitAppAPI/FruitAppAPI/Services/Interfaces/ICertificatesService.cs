using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FruitAppAPI.Services.Interfaces
{
    public interface ICertificatesService
    {
        Task CreateCertificates(List<string> certificates);
        Task FindAndRelate(string nodeId, string certName);
        Task<IEnumerable<string>> GetCertificates();
    }
}
