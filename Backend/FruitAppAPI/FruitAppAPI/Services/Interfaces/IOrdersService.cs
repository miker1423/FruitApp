using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using FruitAppAPI.NeoModels;
using FruitAppAPI.ViewModels;
using FruitAppAPI.ViewModels.Orders;

namespace FruitAppAPI.Services.Interfaces
{
    public interface IOrdersService
    {
        Task<bool> Create(OrderCreateVM orderCreateVM);
        Task<bool> Delete();
        Task<bool> Update();
        Task<bool> Get();
    }
}
