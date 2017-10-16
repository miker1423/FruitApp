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
    public class CertificatesController : Controller
    {
        private readonly ICertificatesService _certificatesService;
        public CertificatesController(ICertificatesService certificatesService)
        {
            _certificatesService = certificatesService;
        }

        [HttpGet]
        public async Task<IActionResult> Get() 
            => Json(await _certificatesService.GetCertificates());
    }
}
