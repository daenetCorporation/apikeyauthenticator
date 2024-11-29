using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Security.Claims;

namespace TestRestApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
         };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet()]
        public IEnumerable<WeatherForecast> GetAnonymous()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpGet()]
        [Route("authorized")]
        [Authorize(AuthenticationSchemes = "ApiKey")]
        public IEnumerable<WeatherForecast> GetAuthorized()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }

        /// <summary>
        /// This method demonstrates impersonation. The client must send the header "ImpersonateTo"
        /// (you can change this inside <see cref="nameof(ClaimsBuilder)"/>), which contains the name 
        /// of the principal (user), for which the Claims Principal will be created.
        /// </summary>
        /// <returns></returns>
        [HttpGet()]
        [Route("impersonated")]
        [Authorize(AuthenticationSchemes = "ApiKey")]
        public IEnumerable<WeatherForecast> GetImpersonated()
        {
            string? impersonatedTo = this.User?.Identity?.Name;
            string? impersonatedFrom = this?.User?.Claims.FirstOrDefault(c => c.Type == "ImpersonatedFrom")?.Value;

            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }


        [HttpGet()]
        [Route("anonymous")]
        [AllowAnonymous()]
        [Authorize(Roles = "admin, contributor")]
        public string Ping()
        {
            return DateTime.UtcNow.ToString(); 
        }
    }
}