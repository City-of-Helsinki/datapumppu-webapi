using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api")]
    public class WebApiController : ControllerBase
    {

        [HttpGet]
        [Route("buttontext")]
        public IActionResult Get()
        {
            return new OkObjectResult(new { text = "button text from the api" });
        }

        [HttpGet]
        [Route("health")]
        public IActionResult HealthCheck()
        {
            return Ok();
        }

    }
}