using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using FruitAppAPI.Services.Interfaces;
using FruitAppAPI.ViewModels.Orders;

namespace FruitAppAPI.Areas.Api.Controllers
{
    [Area("api")]
    [Route("api/[controller]")]
    public class OrdersController : Controller
    {
        private readonly IOrdersService _orderService;
        public OrdersController(IOrdersService ordersService)
        {
            _orderService = ordersService;
        }

        [HttpPost]
        public IActionResult Post([FromBody]OrderCreateVM orderCreateVM) =>
            Json(_orderService.FindProviders(orderCreateVM.Fruit, orderCreateVM.Certificates));
    }
}
