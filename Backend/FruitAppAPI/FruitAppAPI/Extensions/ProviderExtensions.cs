using FruitAppAPI.Models;
using FruitAppAPI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FruitAppAPI.Extensions
{
    public static class ProviderExtensions
    {
        public static Provider ToEntity(this ProviderVM providerVM)
        {
            return new Provider
            {
                Id = providerVM.Id,
                LastName = providerVM.LastName,
                Latitude = providerVM.Latitude,
                Longitude = providerVM.Longitude,
                MiddleName = providerVM.MiddleName,
                Name = providerVM.Name,
                PhoneNumber = providerVM.PhoneNumber
            };
        }
    }
}
