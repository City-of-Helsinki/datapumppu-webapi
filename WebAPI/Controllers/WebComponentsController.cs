using Microsoft.AspNetCore.Mvc;
using System.Text;
using WebAPI.Controllers.Filters;

namespace WebAPI.Controllers
{
    /// <summary>
    /// Controller for serving dynamically configured JavaScript web components.
    /// </summary>
    [ApiController]
    [Route("components")]
    [TypeFilter(typeof(WebAPIExceptionFilter))]
    public class WebComponentsController : ControllerBase
    {

        private readonly IConfiguration _configuration;
        private ILogger<WebComponentsController> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="WebComponentsController"/> class.
        /// </summary>
        /// <param name="configuration">The application configuration.</param>
        /// <param name="logger">The logger instance.</param>
        public WebComponentsController(IConfiguration configuration,
            ILogger<WebComponentsController> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        /// <summary>
        /// Serves the meeting web component JavaScript file with injected configuration values.
        /// </summary>
        /// <param name="year">The meeting year.</param>
        /// <param name="sequenceNumber">The meeting sequence number.</param>
        /// <param name="lang">The language code (en, fi, or sv).</param>
        /// <returns>The configured JavaScript file, or 500 on invalid parameters.</returns>
        [HttpGet]
        [Route("meeting.js")]
        public async Task<IActionResult> GetMeeting(string year, string sequenceNumber, string lang)
        {
            _logger.LogInformation("GET meeting.js {0} {1} {2}", year, sequenceNumber, lang);

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
            if (lowerLang != "en" && lowerLang != "fi" && lowerLang != "sv")
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