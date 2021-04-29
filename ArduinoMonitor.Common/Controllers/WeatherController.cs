using System;
using System.Linq;
using System.Net.Http;
using ArduinoMonitor.Common.Models;
using HtmlAgilityPack;

namespace ArduinoMonitor.Common.Controllers
{
    public static class WeatherController
    {
        private const int PRESSURE_NODE = 4;
        private const int HUMIDITY_NODE = 5;
        private const int WIND_NODE = 6;
        private const int PRECIPITATION_NODE = 7;

        public static WeatherInfo GetWeather()
        {
            var client = new HttpClient();
            try
            {
                var html = client
                    .GetAsync(
                        "https://sinoptik.ua/%D0%BF%D0%BE%D0%B3%D0%BE%D0%B4%D0%B0-%D1%85%D0%B0%D1%80%D1%8C%D0%BA%D0%BE%D0%B2")
                    .Result.Content.ReadAsStringAsync().Result;

                var document = new HtmlDocument();
                document.LoadHtml(html);

                var values = document.DocumentNode
                    .SelectNodes("//td")
                    .Where(x => x.HasClass("cur"))
                    .Select(x => x.InnerText
                        .Replace("&deg;", "")
                        .Replace(" ", ""))
                    .ToList();

                var temperature = document.DocumentNode
                                      .SelectNodes("//p")
                                      .First(x => x.HasClass("today-temp"))?
                                      .InnerText.Replace("&deg;C", "") ?? "~";

                return new WeatherInfo
                {
                    Temperature = temperature,
                    Humidity = values[HUMIDITY_NODE],
                    PrecipitationProbability = values[PRECIPITATION_NODE],
                    Pressure = values[PRESSURE_NODE],
                    Wind = values[WIND_NODE]
                };
            }
            catch
            {
                // TODO: Add Logging
                throw new Exception("Cant get weather");
            }
        }
    }
}