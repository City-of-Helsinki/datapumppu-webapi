using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("components")]
    public class WebComponentsController : ControllerBase
    {

        private readonly IConfiguration _configuration;

        public WebComponentsController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        [Route("example.js")]
        public async Task<IActionResult> GetExample()
        {
            var text = await System.IO.File
                .ReadAllTextAsync("./ScriptFiles/components/example.js");
            var apiUrl = _configuration["API_URL"];
            
            if (apiUrl == null) 
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            text = text.Replace("#--API_URL--#", apiUrl);

            return File(Encoding.UTF8.GetBytes(text), "application/javascript");
        }

        [HttpGet]
        [Route("signalrexample.js")]
        public async Task<IActionResult> GetSignalRExample()
        {
            var text = await System.IO.File
                .ReadAllTextAsync("./ScriptFiles/components/signalrexample.js");
            var apiUrl = _configuration["API_URL"];

            if (apiUrl == null) 
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            text = text.Replace("#--API_URL--#", apiUrl);

            return File(Encoding.UTF8.GetBytes(text), "application/javascript");
        }

        [HttpGet]
        [Route("meeting.js")]
        public async Task<IActionResult> GetMeeting()
        {
            var text = await System.IO.File
                .ReadAllTextAsync("./ScriptFiles/components/meeting.js");
            var apiUrl = _configuration["API_URL"];

            if (apiUrl == null) 
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            text = text.Replace("#--API_URL--#", apiUrl);

            return File(Encoding.UTF8.GetBytes(text), "application/javascript");
        }

        [HttpGet]
        [Route("decision.js")]
        public async Task<IActionResult> GetDecision()
        {
            var text = await System.IO.File
                .ReadAllTextAsync("./ScriptFiles/components/decision.js");
            var apiUrl = _configuration["API_URL"];

            if (apiUrl == null) 
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            text = text.Replace("#--API_URL--#", apiUrl);
            
            return File(Encoding.UTF8.GetBytes(text), "application/javascript");
        }
    }
}