using CurrencyApp.Application.Services;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<IActionResult> Get(
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

            var result = await _service.GetStats(from, to, start, stop);
            
            if (result == null)
                return NoContent();

            return Ok(result);
        }
    }
}
