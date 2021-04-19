using System;

namespace Documented.Api.V1.Models
{
    public class WeatherForecast
    {
        /// <summary>
        /// The date of the forecast, local time
        /// </summary>
        /// <example>2021-04-17T14:22:39.3973797-03:00</example>
        public DateTime Date { get; set; }

        /// <summary>
        /// Forecasted temperature in Celsius
        /// </summary>
        /// <example>22</example>
        public int TemperatureC { get; set; }

        /// <summary>
        /// Forecasted temperature in Fahrenheit rounded
        /// </summary>
        /// <example>72</example>
        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

        /// <summary>
        /// Friendly temperature description
        /// </summary>
        /// <example>Warm</example>
        public string Summary { get; set; }
    }
}
