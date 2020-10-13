using System;
using System.ComponentModel.DataAnnotations;

namespace DotNetLambda
{
    /// <summary>
    /// A weather forecast.
    /// </summary>
    public class WeatherForecast
    {
        /// <summary>
        /// The identifier for the forecast.
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// The date the forecast is for.
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// The forecast temperature in Celsius.
        /// </summary>
        public int TemperatureC { get; set; }

        /// <summary>
        /// The forecast temperature in Fahrenheit.
        /// </summary>
        public int TemperatureF => 32 + (int) (TemperatureC / 0.5556);

        /// <summary>
        /// The summary description of the forecast.
        /// </summary>
        public string Summary { get; set; }
    }
}