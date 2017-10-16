using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FruitAppAPI.ViewModels.Account
{
    public class RegisterVM
    {
        public string Email { get; set; }
        public string Nickname { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }
}
