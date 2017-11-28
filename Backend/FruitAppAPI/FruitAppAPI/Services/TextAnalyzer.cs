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
        public TextAnalyzer(LuisClient luisClient)
        {
            _luisClient = luisClient;
        }

        public async Task<bool> IsYes(string message)
        {
            var result = await _luisClient.Predict(message);
            return result.TopScoringIntent.Name == "YES";
        }
    }
}
