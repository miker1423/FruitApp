using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using FruitAppAPI.Services.Interfaces;
using FruitAppAPI.Models;

namespace FruitAppAPI.Api.Controllers
{
    [Area("api")]
    [Route("api/[controller]")]
    public class ProvidersController : Controller
    {
        private readonly IProviderService _providerService;

        public ProvidersController(IProviderService providerService)
        {
            _providerService = providerService;
        }

        [HttpGet]
        public async Task<IActionResult> Get() => Json(await _providerService.GetProviders());

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            if(Guid.TryParse(id, out var providerId))
            {
                return Json(await _providerService.GetProvider(providerId));
            }

            return BadRequest("The given ID is not a Guid");
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]Provider provider)
        {
            try
            {
                var id = await _providerService.CreateProvider(provider);
                return Json(new { Id = id.ToString() });
            }
            catch (Exception ex)
            {
                return BadRequest($"Something went wrong {ex.Message}");
            }
        }

        [HttpPut]
        public async Task<IActionResult> Put([FromBody]Provider provider)
        {
            try
            {
                await _providerService.UpdateProvider(provider);
                return Accepted();
            }
            catch (Exception ex)
            {
                return BadRequest($"Something went wrong {ex.Message}");
            }
        }
    }
}
