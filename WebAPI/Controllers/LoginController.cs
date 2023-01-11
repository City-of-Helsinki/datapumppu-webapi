using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using WebAPI.Controllers.DTOs;
using WebAPI.Controllers.Filters;
using WebAPI.StorageClient;

namespace WebAPI.Controllers
{
    [ApiController]
    [TypeFilter(typeof(WebAPIExceptionFilter))]
    public class LoginController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<LoginController> _logger;
        private readonly IStorageApiClient _storageApiClient;

        public LoginController(IConfiguration configuration,
            ILogger<LoginController> logger,
            IStorageApiClient storageApiClient)
        {
            _configuration = configuration;
            _logger = logger;
            _storageApiClient = storageApiClient;
        }

        [AllowAnonymous]
        [Route("login")]
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] UserDTO userLogin)
        {
            _logger.LogInformation("Executing Login()");

            if (await _storageApiClient.CheckLogin(userLogin.Username, userLogin.Password))
            {
                var token = GenerateToken();
                return Ok(new { token });
            }
            return Forbid();
        }

        private string GenerateToken()
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT_KEY"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(_configuration["JWT_ISSUER"],
                _configuration["JWT_AUDIENCE"],
                expires: DateTime.Now.AddMinutes(60),
                signingCredentials: credentials);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [HttpGet]
        [Route("logout")]
        [Authorize]
        public ActionResult Logout()
        {
            return Ok();
        }
    }
};
