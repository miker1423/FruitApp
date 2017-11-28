using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using FruitAppAPI.NeoModels;
using FruitAppAPI.ViewModels;
using FruitAppAPI.ViewModels.Orders;
using Microsoft.WindowsAzure.Storage.Table;

namespace FruitAppAPI.Services.Interfaces
{
    public interface IOrdersService
    {
        Task<Guid> Create(OrderCreateVM orderCreateVM);
        Task<FullOrderVM> Get(Guid id);
        Task<List<GetOrderVM>> Get();
        Task UpdateOrderWithPhone(string phone);
    }
}
