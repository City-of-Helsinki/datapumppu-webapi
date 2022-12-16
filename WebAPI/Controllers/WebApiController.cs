using System.Text;
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
        [Route("translations")]
        public async Task<IActionResult> GetTranslations(string lang)
        {
            var text = await System.IO.File
                .ReadAllTextAsync("./Resources/"+lang+"/translation.json");     
            return File(Encoding.UTF8.GetBytes(text), "application/json");
        }

        [HttpGet]
        [Route("health")]
        public IActionResult HealthCheck()
        {
            return Ok();
        }

    }
}