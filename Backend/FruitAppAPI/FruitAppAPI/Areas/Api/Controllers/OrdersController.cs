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
        public async Task<IActionResult> Post([FromBody]OrderCreateVM orderCreateVM) 
            => CreatedAtAction(nameof(this.Get), new { ID = await _orderService.Create(orderCreateVM) });

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            if(Guid.TryParse(id, out var ID))
            {
                return Json(await _orderService.Get(ID));
            }

            return BadRequest("The provided ID is not valid");
        }

        [HttpGet]
        public async Task<IActionResult> Get() 
            => Json(await _orderService.Get());
    }
}
