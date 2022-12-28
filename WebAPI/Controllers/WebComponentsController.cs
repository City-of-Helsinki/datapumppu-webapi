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

            text = text.Replace("#--API_URL--#", apiUrl);
            text = text.Replace("#--MEETING_YEAR--#", year);
            text = text.Replace("#--MEETING_SEQUENCE_NUM--#", sequenceNumber);
            text = text.Replace("#--LANGUAGE--#", lang);
            return File(Encoding.UTF8.GetBytes(text), "application/javascript");
        }

        [HttpGet]
        [Route("decision.js")]
        public async Task<IActionResult> GetDecision(string caseIdLabel, string lang)
        {
            _logger.LogInformation("GET decision.js");

            var text = await System.IO.File
                .ReadAllTextAsync("./ScriptFiles/components/decision.js");
            var apiUrl = _configuration["API_URL"];

            if (apiUrl == null) 
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            //text = text.Replace("#--API_URL--#", apiUrl);0
            text = text.Replace("#--API_URL--#", apiUrl);
            text = text.Replace("#--CASE_ID_LABEL--#", caseIdLabel);
            text = text.Replace("#--LANG--#", lang);


            return File(Encoding.UTF8.GetBytes(text), "application/javascript");
        }
    }
}