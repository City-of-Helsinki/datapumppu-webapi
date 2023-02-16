using Microsoft.AspNetCore.Mvc;
using System.Text;
using WebAPI.Controllers.Filters;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("components")]
    [TypeFilter(typeof(WebAPIExceptionFilter))]
    public class WebComponentsController : ControllerBase
    {

        private readonly IConfiguration _configuration;
        private ILogger<WebComponentsController> _logger;

        public WebComponentsController(IConfiguration configuration,
            ILogger<WebComponentsController> logger)
        {
            _configuration = configuration;
            _logger = logger;
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
        public async Task<IActionResult> GetMeeting(string year, string sequenceNumber, string lang)
        {
            _logger.LogInformation("GET meeting.js");

            var text = await System.IO.File
                .ReadAllTextAsync("./ScriptFiles/components/meeting.js");
            var apiUrl = _configuration["API_URL"];

            if (apiUrl == null) 
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            if (!Int32.TryParse(year, out int yearInt))
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            if (!Int32.TryParse(sequenceNumber, out int sequenceNumberInt))
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            string lowerLang = lang.ToLower();
            if (lowerLang != "en" && lowerLang != "fi" && lowerLang == "sv")
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            text = text.Replace("#--API_URL--#", apiUrl);
            text = text.Replace("#--MEETING_YEAR--#", yearInt.ToString());
            text = text.Replace("#--MEETING_SEQUENCE_NUM--#", sequenceNumberInt.ToString());
            text = text.Replace("#--LANGUAGE--#", lowerLang);
            return File(Encoding.UTF8.GetBytes(text), "application/javascript");
        }
    }
}