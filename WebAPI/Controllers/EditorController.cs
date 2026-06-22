using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using WebAPI.Controllers.DTOs;
using WebAPI.Controllers.Filters;
using WebAPI.Data;
using WebAPI.StorageClient;

namespace WebAPI.Controllers
{
    /// <summary>
    /// Controller for editor authentication and content editing operations.
    /// Provides JWT-based login and authorized endpoints for agenda point and video sync updates.
    /// </summary>
    [ApiController]
    [Route("editor")]
    [TypeFilter(typeof(WebAPIExceptionFilter))]
    public class EditorController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EditorController> _logger;
        private readonly IStorageApiClient _storageApiClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="EditorController"/> class.
        /// </summary>
        /// <param name="configuration">The application configuration.</param>
        /// <param name="logger">The logger instance.</param>
        /// <param name="storageApiClient">The storage API client.</param>
        public EditorController(IConfiguration configuration,
            ILogger<EditorController> logger,
            IStorageApiClient storageApiClient)
        {
            _configuration = configuration;
            _logger = logger;
            _storageApiClient = storageApiClient;
        }

        /// <summary>
        /// Authenticates an editor user and returns a JWT token.
        /// </summary>
        /// <param name="userLogin">The login credentials.</param>
        /// <returns>A JWT token on success, or 403 Forbidden on failure.</returns>
        [AllowAnonymous]
        [Route("login")]
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] UserDTO userLogin)
        {
            _logger.LogInformation($"Executing Login() {userLogin.Username}");

            await Task.Delay(1000);
            if (await _storageApiClient.CheckLogin(userLogin.Username, userLogin.Password))
            {
                _logger.LogInformation($"Login success {userLogin.Username}");
                var token = GenerateToken(userLogin.Username);
                return Ok(new { token });
            }

            _logger.LogInformation($"Login failed {userLogin.Username}");
            await Task.Delay(5000);

            return Forbid();
        }

        /// <summary>
        /// Updates an agenda point. Requires authorization.
        /// </summary>
        /// <param name="editItem">The agenda point edit data.</param>
        /// <returns>200 OK on success, or 403 Forbidden on failure.</returns>
        [HttpPost("edit")]
        [Authorize]
        public async Task<IActionResult> UpdateAgendaPoint([FromBody] EditAgendaPointDTO editItem)
        {
            var userNameClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name);
            _logger.LogInformation($"UpdateAgendaPoint {editItem.MeetingId}/{editItem.AgendaPoint} ({userNameClaim?.Value})");
            editItem.EditorUserName = userNameClaim?.Value ?? string.Empty;

            return await _storageApiClient.UpdateAgendaPoint(editItem) ? Ok() : StatusCode(StatusCodes.Status403Forbidden);
        }

        /// <summary>
        /// Updates the video synchronization position. Requires authorization.
        /// </summary>
        /// <param name="videoSync">The video sync data.</param>
        /// <returns>200 OK on success.</returns>
        [HttpPost("videosync")]
        [Authorize]
        public async Task<IActionResult> UpdateVideoSync([FromBody] VideoSyncDTO videoSync)
        {
            _logger.LogInformation($"UpdateVideoSync {videoSync.MeetingID}/{videoSync.VideoPosition}");
            await _storageApiClient.UpdateVideoSync(videoSync);
            return Ok();
        }

        /// <summary>
        /// Logs out the current editor user. Requires authorization.
        /// </summary>
        /// <returns>200 OK.</returns>
        [HttpGet]
        [Route("logout")]
        [Authorize]
        public ActionResult Logout()
        {
            return Ok();
        }

        private string GenerateToken(string userName)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT_KEY"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(_configuration["JWT_ISSUER"],
                _configuration["JWT_AUDIENCE"],
                new List<Claim>
                {
                    new Claim(ClaimTypes.Name, userName)
                },
                expires: DateTime.Now.AddHours(12),
                signingCredentials: credentials);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
};
