using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Twilio;
using Twilio.Types;

using FruitAppAPI.Services.Interfaces;
using Twilio.Rest.Api.V2010.Account;

namespace FruitAppAPI.Services
{
    public class SmsService : ISmsService
    {
        PhoneNumber myNumber;
        public SmsService()
        {
            const string accoundtSid = "";
            const string authToken = "";

            TwilioClient.Init(accoundtSid, authToken);

            myNumber = new PhoneNumber("");
        }

        public async Task<string> SendMessage(string message, string phone)
        {
            var number = new PhoneNumber(phone);
            var sentMessage = await MessageResource.CreateAsync(number, body: message);

            return sentMessage.Sid;
        }
    }
}
