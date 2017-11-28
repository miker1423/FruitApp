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
            const string accoundtSid = "AC7039b3c3079a057849417c9deb8ed26c";
            const string authToken = "bdce9a45d3693dd6876cb041992e900a";

            TwilioClient.Init(accoundtSid, authToken);

            myNumber = new PhoneNumber("+14154170925");
        }

        public async Task<string> SendMessage(string message, string phone)
        {
            var number = new PhoneNumber(phone);
            var sentMessage = await MessageResource.CreateAsync(number, body: message, from: myNumber);

            return sentMessage.Sid;
        }
    }
}
