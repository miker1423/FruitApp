using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Text;
using System.Linq;

using Twilio.AspNet.Core;
using Twilio.TwiML;
using FruitAppAPI.Services.Interfaces;

namespace FruitAppAPI.Areas.Api.Controllers
{
    [Area("api")]
    [Route("api/[controller]")]
    public class SmsController : TwilioController
    {
        private readonly IOrdersService _ordersService;
        private readonly ITextAnalyzer _textAnalyzer;

        public SmsController(
            ITextAnalyzer textAnalyzer,
            IOrdersService ordersService)
        {
            _ordersService = ordersService;
            _textAnalyzer = textAnalyzer;
        }

        [HttpPost]
        public async Task<IActionResult> Post()
        {
            var response = new MessagingResponse();

            var size = HttpContext.Request.ContentLength ?? 0;
            var buffer = new byte[size];
            var body = await HttpContext.Request.Body.ReadAsync(buffer, 0, (int)size);
            var responseDictionary = Encoding.UTF8.GetString(buffer)
                .Split('&')
                .Select(str => str.Split('='))
                .ToDictionary(arr => arr[0], arr2 => arr2[1]);

            var number = responseDictionary["From"].Substring(3);
            var newNumber = "+" + number;

            if(await _textAnalyzer.IsYes(responseDictionary["Body"])) await _ordersService.UpdateOrderWithPhone(newNumber);

            return TwiML(response);
        }  
    }

    public class TwilioMessage
    {
        public string MessageSid { get; set; }
        public string SmsSid { get; set; }
        public string AccountSid { get; set; }
        public string MessagingServiceSid { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string Body { get; set; }
        public int NumMedia { get; set; }
    }
}
