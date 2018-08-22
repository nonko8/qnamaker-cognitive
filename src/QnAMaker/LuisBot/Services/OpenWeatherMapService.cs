using System.Net.Http;
using System.Configuration;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System;
using BotApp.Models;

namespace BotApp.Services
{
    [Serializable]
    public class OpenWeatherMapService
    {
        private static readonly string OpenWeatherBaseurl = "http://api.openweathermap.org/data/2.5/";

        private static readonly string ImgBaseurl = "http://openweathermap.org/img/w/";

        private static readonly string Query = "/data/2.5/weather?q={0}&units={1}&lang={2}";

        private string ApiKey = "";

        public OpenWeatherMapService()
        {
            ApiKey = ConfigurationManager.AppSettings["OpenWeatherAPIKey"];
        }

        public async Task<WeatherInfo> GetWeatherAsync(WeatherQuery weatherQuery)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(OpenWeatherBaseurl);
                httpClient.DefaultRequestHeaders.Add("x-api-key", this.ApiKey);
                string url = string.Format(Query, weatherQuery.Location, "Metric", "jp");
                HttpResponseMessage response = await httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    JsonSerializerSettings settings = new JsonSerializerSettings();
                    settings.Culture = new System.Globalization.CultureInfo("en-us");
                    return JsonConvert.DeserializeObject<WeatherInfo>(json, settings);
                }
                else
                {
                    return null;
                }
            }
        }

        public string GetWeatherIcon(WeatherInfo weatherInfo)
        {
            string weatherIcon = "";
            weatherInfo.Weather.ForEach(w => { weatherIcon = w.Icon; });
            return ImgBaseurl + $"{weatherIcon}.png";
        }
    }
}