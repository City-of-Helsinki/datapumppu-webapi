using System.Text;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Controllers.Filters;

namespace WebAPI.Controllers
{
    /// <summary>
    /// Controller for retrieving UI translation files.
    /// </summary>
    [ApiController]
    [Route("api")]
    [TypeFilter(typeof(WebAPIExceptionFilter))]
    public class TranslationsController : ControllerBase
    {
        /// <summary>
        /// Retrieves the translation JSON file for the specified language.
        /// </summary>
        /// <param name="lang">The language code (en, fi, or sv).</param>
        /// <returns>The translation JSON content.</returns>
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