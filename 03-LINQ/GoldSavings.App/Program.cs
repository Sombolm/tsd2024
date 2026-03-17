using GoldSavings.App.Model;
using GoldSavings.App.Client;
using GoldSavings.App.Services;
using System.Xml.Serialization;
using System.IO;
using System.Xml;
namespace GoldSavings.App;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Hello, Gold Investor!");

        GoldDataService dataService = new GoldDataService();
        DateTime startDate = new DateTime(2019, 1, 1);
        DateTime endDate = DateTime.Now;
        List<GoldPrice> goldPrices = dataService.GetGoldPrices(startDate, endDate).GetAwaiter().GetResult();

        if (goldPrices.Count == 0)
        {
            Console.WriteLine("No data found. Exiting.");
            return;
        }

        Console.WriteLine($"Retrieved {goldPrices.Count} records. Ready for analysis.");

        AnswerAllQuestions(goldPrices);

        //Task 3 and 4:
        string xmlFilePath = "goldPrices.xml";

        SaveToXml(goldPrices, xmlFilePath);

        Console.WriteLine("\n=== Reading data from XML file ===");
        List<GoldPrice>? loadedPrices = ReadFromXml(xmlFilePath);

        if (loadedPrices != null)
        {
            Console.WriteLine($"Successfully loaded {loadedPrices.Count} records from XML!");
        }

        Console.WriteLine("\nGold Analyis Queries with LINQ Completed.");
    }

    static void AnswerAllQuestions(List<GoldPrice> goldPrices)
    {
        AnswerQuestionA(goldPrices);
        AnswerQuestionB(goldPrices);
        AnswerQuestionC(goldPrices);
        AnswerQuestionD(goldPrices);
        AnswerQuestionE(goldPrices);
    }

    static void AnswerQuestionA(List<GoldPrice> goldPrices)
    {
        Console.WriteLine("=== What are the TOP 3 highest and TOP 3 lowest prices ofgold within the last year? ===");

        DateTime oneYearAgo = DateTime.Now.AddYears(-1);
        var lastYearPrices = goldPrices.Where(p => p.Date >= oneYearAgo).ToList();

        var top3HighestMethod = lastYearPrices.OrderByDescending(p => p.Price).Take(3).ToList();
        var top3LowestMethod = lastYearPrices.OrderBy(p => p.Price).Take(3).ToList();

        var top3HighestQuery = (from p in lastYearPrices
                                orderby p.Price descending
                                select p).Take(3).ToList();

        var top3LowestQuery = (from p in lastYearPrices
                               orderby p.Price ascending
                               select p).Take(3).ToList();

        GoldResultPrinter.PrintPrices(top3HighestMethod, "TOP 3 Highest (Query/Method Syntax)");
        GoldResultPrinter.PrintPrices(top3HighestQuery, "TOP 3 Highest (Query/Method Syntax)");

        GoldResultPrinter.PrintPrices(top3LowestMethod, "TOP 3 Lowest (Query/Method Syntax)");
        GoldResultPrinter.PrintPrices(top3LowestQuery, "TOP 3 Lowest (Query/Method Syntax)");
    }

static void AnswerQuestionB(List<GoldPrice> goldPrices)
    {
        Console.WriteLine("\n=== If one had bought gold in January 2020, is it possible that they would have earned more than 5%? On which days? ===");

        var firstDayJan2020 = goldPrices.Where(p => p.Date.Year == 2020 && p.Date.Month == 1)
                                        .OrderBy(p => p.Date)
                                        .FirstOrDefault();

        if (firstDayJan2020 != null)
        {
            double buyPrice = firstDayJan2020.Price;
            double targetPrice = buyPrice * 1.05;

            var profitableDays = goldPrices.Where(p => p.Date > firstDayJan2020.Date && p.Price > targetPrice).ToList();

            Console.WriteLine($"Is it possible? {(profitableDays.Any() ? "YES" : "NO")}");
            Console.WriteLine("\n Profitable Days: ");

            if (profitableDays.Any())
            {
                GoldResultPrinter.PrintPrices(profitableDays, "Profitable Days");
            }
        }
    }

    static void AnswerQuestionC(List<GoldPrice> goldPrices)
    {
        Console.WriteLine("\n=== Which 3 dates of 2022-2019 opens the second ten of the prices ranking? ===");

        var secondTenOpeners = goldPrices.Where(p => p.Date.Year >= 2019 && p.Date.Year <= 2022)
                                         .OrderByDescending(p => p.Price)
                                         .Skip(10)
                                         .Take(3)
                                         .ToList();

        if (secondTenOpeners.Any())
        {
            GoldResultPrinter.PrintPrices(secondTenOpeners, "Positions 11, 12, 13 in the ranking (2019-2022)");
        }
        else
        {
            Console.WriteLine("Not enough data");
        }
    }

    static void AnswerQuestionD(List<GoldPrice> goldPrices)
    {
        Console.WriteLine("\n=== What are the averages of gold prices in 2020, 2023, 2024? (Query Syntax) ===");

        var avg2020 = (from p in goldPrices
                       where p.Date.Year == 2020
                       select p.Price).Average();

        var avg2023 = (from p in goldPrices
                       where p.Date.Year == 2023
                       select p.Price).Average();

        var avg2024 = (from p in goldPrices
                       where p.Date.Year == 2024
                       select p.Price).Average();

        Console.WriteLine($"Average price in 2020: {Math.Round(avg2020, 2)} PLN");
        Console.WriteLine($"Average price in 2023: {Math.Round(avg2023, 2)} PLN");
        Console.WriteLine($"Average price in 2024: {Math.Round(avg2024, 2)} PLN");
    }

    static void AnswerQuestionE(List<GoldPrice> goldPrices)
    {
        Console.WriteLine("\n=== When it would be best to buy gold and sell it between 2020 and 2024? What would be the return on investment? ===");

        var periodPrices = goldPrices.Where(p => p.Date.Year >= 2020 && p.Date.Year <= 2024).ToList();

        var bestTrade = (from buy in periodPrices
                         from sell in periodPrices
                         where sell.Date > buy.Date
                         orderby (sell.Price - buy.Price) descending
                         select new
                         {
                             BuyDate = buy.Date,
                             BuyPrice = buy.Price,
                             SellDate = sell.Date,
                             SellPrice = sell.Price,
                             Profit = sell.Price - buy.Price,
                             ROI = ((sell.Price - buy.Price) / buy.Price) * 100
                         }).FirstOrDefault();

        if (bestTrade != null)
        {
            Console.WriteLine($"Best to BUY: {bestTrade.BuyDate:yyyy-MM-dd} (Price: {bestTrade.BuyPrice} PLN)");
            Console.WriteLine($"Best to SELL: {bestTrade.SellDate:yyyy-MM-dd} (Price: {bestTrade.SellPrice} PLN)");
            Console.WriteLine($"Return on Investment: {Math.Round(bestTrade.ROI, 2)}%");
        }
        else
        {
            Console.WriteLine("Not enough data to find a profitable trade.");
        }
    }

    static void SaveToXml(List<GoldPrice> goldPrices, string filePath)
    {
        Console.WriteLine("\n=== Saving data to XML ===");

        XmlSerializer serializer = new XmlSerializer(typeof(List<GoldPrice>));

        using (StreamWriter writer = new StreamWriter(filePath))
        {
            serializer.Serialize(writer, goldPrices);
        }

        Console.WriteLine("Data successfully saved to XML.");
    }

    static List<GoldPrice>? ReadFromXml(string filePath) =>
        (List<GoldPrice>?)new XmlSerializer(typeof(List<GoldPrice>)).Deserialize(XmlReader.Create(filePath));
}
