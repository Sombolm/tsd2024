using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GoldSavings.App.Client;
using GoldSavings.App.Model;

namespace GoldSavings.App.Services
{
    public class GoldDataService
    {
        private readonly GoldClient _goldClient;

        public GoldDataService()
        {
            _goldClient = new GoldClient();
        }

        public async Task<List<GoldPrice>> GetGoldPrices(DateTime startDate, DateTime endDate)
        {
            List<GoldPrice> allPrices = new List<GoldPrice>();
            DateTime currentStart = startDate;

            while (currentStart <= endDate)
            {
                DateTime currentEnd = currentStart.AddDays(92); 
                if (currentEnd > endDate)
                {
                    currentEnd = endDate;
                }

                var pricesChunk = await _goldClient.GetGoldPrices(currentStart, currentEnd);
                
                if (pricesChunk != null)
                {
                    allPrices.AddRange(pricesChunk);
                }

                currentStart = currentEnd.AddDays(1);
            }

            return allPrices;
        }
    }
}
