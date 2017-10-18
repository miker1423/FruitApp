using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using FruitAppAPI.DBContexts;
using FruitAppAPI.Models;

namespace FruitAppAPI.Areas.Api.Controllers
{
    [Area("api")]
    [Route("api/[controller]")]
    public class ClientsController : Controller 
    {
        private readonly AppDBContext _context;

        public ClientsController(AppDBContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Get() => Json(_context.Clients);

        [HttpGet("{id}")]
        public IActionResult Get(string id)
        {
            if(Guid.TryParse(id, out var ID))
            {
                return Json(_context.Clients.Where(client => client.Id == ID).First());
            }

            return BadRequest("The given ID is not a GUID");
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]Client client)
        {
            client.Id = Guid.NewGuid();
            _context.Clients.Add(client);
            await _context.SaveChangesAsync();

            return Json(new { Id = client.Id });
        }

        [HttpPut]
        public async Task<IActionResult> Put([FromBody]Client client)
        {
            _context.Clients.Update(client);
            await _context.SaveChangesAsync();
            return Json(client);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if(Guid.TryParse(id, out var ID))
            {
                var client = _context.Clients.Where(lookup => lookup.Id == ID).First();
                var removedClient = _context.Clients.Remove(client);
                await _context.SaveChangesAsync();

                return Ok();
            }

            return BadRequest("The provided id is not a GUID");
        }
    }
}
