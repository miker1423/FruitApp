using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FruitAppAPI.Services.Interfaces
{
    public interface ISmsService
    {
        Task<string> SendMessage(string message, string phone);
    }
}
