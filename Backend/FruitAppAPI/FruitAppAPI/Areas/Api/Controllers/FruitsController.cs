using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using FruitAppAPI.Services.Interfaces;

namespace FruitAppAPI.Api.Controllers
{
    [Area("api")]
    [Route("api/[controller]")]
    public class FruitsController : Controller
    {
        private readonly IFruitGraphService _fruitGraphService;
        public FruitsController(IFruitGraphService fruitGraphService)
        {
            _fruitGraphService = fruitGraphService;
        }

        [HttpGet]
        public async Task<IActionResult> Get() 
            => Json(await _fruitGraphService.GetFruits());
    }
}
