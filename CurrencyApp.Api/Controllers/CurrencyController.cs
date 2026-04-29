using CurrencyApp.Application.Enums;
using CurrencyApp.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace CurrencyApp.Api.Controllers
{
    [Route("api/currency")]
    [ApiController]
    public class CurrencyController : ControllerBase
    {
        private readonly CurrencyService _service;

        public CurrencyController(CurrencyService service)
        {
            _service = service;
        }

        [HttpGet]
        [EnableRateLimiting("stats-limit")]
        public async Task<IActionResult> Get(
            [FromQuery] CurrencyApiType apiType,
            [FromQuery] string from, 
            [FromQuery] string to, 
            [FromQuery] DateTime? fromDate, 
            [FromQuery] DateTime? toDate)
        {
            if (string.IsNullOrEmpty(from) || string.IsNullOrEmpty(to))
                return BadRequest(new { message = "Currency codes are required" });

            var start = fromDate ?? DateTime.Today;
            var stop = toDate ?? DateTime.Today;

            if (start > DateTime.Today || stop > DateTime.Today)
                return BadRequest(new { message = "Dates cannot be in the future" });

            if (start > stop)
                return BadRequest(new { message = "Invalid date range!" });

            var result = await _service.GetStats(apiType, from, to, start, stop);

            if (!result.IsSuccess)
                return NoContent();

            return Ok(result.Value);
        }

        
        [HttpGet("currencies")]
        public async Task<IActionResult> GetCurrencies([FromQuery] CurrencyApiType apiType)
        {
            var result = await _service.GetCurrencies(apiType);

            return Ok(result);
        }
    }
}
