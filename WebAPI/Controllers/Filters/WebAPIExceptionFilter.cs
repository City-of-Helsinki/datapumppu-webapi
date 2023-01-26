using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;

namespace WebAPI.Controllers.Filters
{
    public class WebAPIExceptionFilter : IExceptionFilter
    {
        private ILogger<WebAPIExceptionFilter> _logger;
        private IHostEnvironment _hostEnvironment;

        public WebAPIExceptionFilter(IHostEnvironment hostEnvironment, ILogger<WebAPIExceptionFilter> logger)
        {
            _hostEnvironment = hostEnvironment;
            _logger = logger;
        }

        public void OnException(ExceptionContext context)
        {
            if (_hostEnvironment.IsDevelopment())
            {
                context.Result = new ContentResult
                {
                    Content = context.Exception.ToString()
                };
            }
            else
            {
                context.Result = new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }

            _logger.LogError($"Exception throw: {context.Exception.ToString()}");
        }
    }
}
