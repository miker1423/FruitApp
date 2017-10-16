using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;

using FruitAppAPI.Models;
using FruitAppAPI.ViewModels.Account;

namespace FruitAppAPI.Api.Controllers
{
    [Area("api")]
    public class AccountController : Controller
    {
        public readonly UserManager<ApplicationUser> _userManager;
        public AccountController(
            UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult Register() => View();

        [HttpGet]
        public IActionResult Success() => View();

        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM registerVM)
        {
            var user = new ApplicationUser
            {
                UserName = registerVM.Nickname,
                Email = registerVM.Email
            };

            var response = await _userManager.CreateAsync(user, registerVM.Password);
            if (response.Succeeded)
            {
                return RedirectToAction(nameof(this.Success));
            }

            registerVM.Password = string.Empty;
            registerVM.ConfirmPassword = string.Empty;
            return View(registerVM);
        }
    }
}
