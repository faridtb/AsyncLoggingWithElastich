using EventBus.Base.Abstraction;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System.Text.Json;
using TestApi.IntegrationEvents.Events;

namespace TestApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestApiController : ControllerBase
    {
        private readonly ILogger<ESLoqIntegrationEvent> _logger;
        
        public TestApiController(ILogger<ESLoqIntegrationEvent> logger)
        {
            _logger= logger;
        }


        [HttpPost("info")]       
        public async Task<IActionResult> Information()
        {
            string data1 = "INFORMATIONIATE";
            string data2 = "MELUMAT";
            string json = JsonSerializer.Serialize(new { Data = data1, Test = data2 }, (JsonSerializerOptions)null);

            _logger.LogInformation(json);           
            return Ok(json);
        }

        [HttpPost("warn")]
        public async Task<IActionResult> Warning()
        {
            string data1 = "WARNING!!";
            string data2 = "XEBERDARLIQ";
            string json = JsonSerializer.Serialize(new { Data = data1, Test = data2 }, (JsonSerializerOptions)null);

            _logger.LogWarning(json);
            return Ok(json);
        }

        [HttpPost("crit")]
        public async Task<IActionResult> Critical()
        {
            string data1 = "CRITICAL!!";
            string data2 = "GEDICI";
            string json = JsonSerializer.Serialize(new { Data = data1, Test = data2 }, (JsonSerializerOptions)null);

            _logger.LogCritical(json);
            return Ok(json);
        }

        [HttpGet("error")]
        public async Task<IActionResult> GetError()
        {
            try
            {
                throw new Exception("Error in api");
            }
            catch (Exception)
            {
                // meselen
                throw;
            }
        }

    }
}
