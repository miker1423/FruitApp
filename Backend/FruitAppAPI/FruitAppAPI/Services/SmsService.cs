using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Twilio;
using Twilio.Types;

using FruitAppAPI.Services.Interfaces;
using Twilio.Rest.Api.V2010.Account;
using Microsoft.Extensions.Options;
using FruitAppAPI.Utils.ConfigModels;

namespace FruitAppAPI.Services
{
    public class SmsService : ISmsService
    {
        PhoneNumber myNumber;
        public SmsService(IOptions<TwilioConfig> config)
        {
            TwilioClient.Init(config.Value.AccountSid, config.Value.AuthToken);

            myNumber = new PhoneNumber(config.Value.PhoneNumber);
        }

        public async Task<string> SendMessage(string message, string phone)
        {
            var number = new PhoneNumber(phone);
            var sentMessage = await MessageResource.CreateAsync(number, body: message, from: myNumber);

            return sentMessage.Sid;
        }
    }
}
