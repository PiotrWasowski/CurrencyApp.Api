using Microsoft.AspNetCore.Mvc;

namespace CurrencyApp.Api.Controllers
{
    [ApiController]
    [Route("api/test")]
    public class TestController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("API działa!");
        }
    }
}
