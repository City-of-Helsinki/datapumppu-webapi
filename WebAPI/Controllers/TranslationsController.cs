using System.Text;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Controllers.Filters;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api")]
    [TypeFilter(typeof(WebAPIExceptionFilter))]
    public class TranslationsController : ControllerBase
    {
        [HttpGet]
        [Route("translations")]
        public async Task<IActionResult> GetTranslations(string lang)
        {
            var text = await System.IO.File
                .ReadAllTextAsync("./Resources/"+lang+"/translation.json");     
            return File(Encoding.UTF8.GetBytes(text), "application/json");
        }
    }
}