using System;
using System.Linq;
using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DotNetLambda.Controllers
{
    /// <summary>
    /// Weather forecast controller.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        /// <summary>
        /// Creates a new instance of the class.
        /// </summary>
        /// <param name="logger">The logger to be used in the class.</param>
        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Gets a list of weather forecasts.
        /// </summary>
        /// <returns>The list of weather forecasts.</returns>
        [HttpGet, EnableQuery]
        public IQueryable<WeatherForecast> Get()
        {
            _logger.LogDebug("Getting a list of weather forecasts.");
            Random rng = new Random();
            return Enumerable.Range(1, 20).Select(index => new WeatherForecast
            {
                Id = index,
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            }).AsQueryable();
        }
    }
}
