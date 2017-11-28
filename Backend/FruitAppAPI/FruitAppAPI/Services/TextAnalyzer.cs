using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Cognitive.LUIS;

using FruitAppAPI.Services.Interfaces;

namespace FruitAppAPI.Services
{
    public class TextAnalyzer : ITextAnalyzer
    {
        private readonly LuisClient _luisClient;
        public TextAnalyzer()
        {
            _luisClient = new LuisClient("37495ff6-5363-4893-ba88-7986ef582910", "9098e287246643ccac9f238365c16bda");
        }

        public async Task<bool> IsYes(string message)
        {
            var result = await _luisClient.Predict(message);
            return result.TopScoringIntent.Name == "YES";
        }
    }
}
