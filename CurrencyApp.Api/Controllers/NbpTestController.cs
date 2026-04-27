using CurrencyApp.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CurrencyApp.Api.Controllers
{
    [Route("api/nbp")]
    [ApiController]
    public class NbpTestController : ControllerBase
    {
        private readonly ICurrencyProvider _provider;

        public NbpTestController(ICurrencyProvider provider)
        {
            _provider = provider;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var rates = await _provider.GetRatesAsync("EUR", "PLN", DateTime.Now.AddDays(-5), DateTime.Today);
                return Ok(rates);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
