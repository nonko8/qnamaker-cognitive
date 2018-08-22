using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotApp.Models
{
    public class Coordinates
    {
        [JsonProperty("lon")]
        public double Longitude { get; set; }

        [JsonProperty("lat")]
        public double Latitude { get; set; }
    }

    public class Sys
    {
        public double? Message { get; set; }
        public string Country { get; set; }

        /*
        [JsonConverter(typeof(Epoch2DateTime))]
        public DateTime? Sunrise
        {
            get;
            set;
        }
        [JsonConverter(typeof(Epoch2DateTime))]
        public DateTime? Sunset
        {
            get;
            set;
        }
        */
        public string pod { get; set; }
    }

    public class Weather
    {
        public int Id { get; set; }
        public string Main { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; }
    }

    public class Main
    {
        [JsonProperty("temp")]
        public double Temperature { get; set; }

        [JsonProperty("pressure")]
        public double Pressure { get; set; }
        [JsonProperty("temp_min")]
        public double? MinTemperature { get; set; }
        [JsonProperty("temp_max")]
        public double? MaxTemperature { get; set; }
        [JsonProperty("humidity")]
        public double Humidity { get; set; }
        public double? sea_level { get; set; }
        public double? grnd_level { get; set; }
        public double? temp_kf { get; set; }

    }

    public class Wind
    {
        public double Speed { get; set; }
        public double? Gust { get; set; }
        public double Deg { get; set; }
    }

    public class Clouds
    {
        public double All { get; set; }
    }

    public class WeatherInfo
    {
        [JsonProperty("coord")]
        public Coordinates Coordinates { get; set; }
        public Sys sys { get; set; }

        public List<Weather> Weather { get; set; }

        [JsonProperty("base")]
        public string Base { get; set; }

        public Main Main { get; set; }

        public Wind Wind { get; set; }

        public Clouds Clouds { get; set; }

        [JsonProperty("dt")]
        public double dt { get; set; }
  
        public int Id { get; set; }
        
        public string Name { get; set; }

        [JsonProperty("cod")]
        public int Code { get; set; }
    }
}
