using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;

namespace WebAPI.Controllers.Filters
{
    /// <summary>
    /// Global exception filter that handles unhandled exceptions in controllers.
    /// Returns full exception details in development mode; returns 500 status in production.
    /// </summary>
    public class WebAPIExceptionFilter : IExceptionFilter
    {
        private ILogger<WebAPIExceptionFilter> _logger;
        private IHostEnvironment _hostEnvironment;

        /// <summary>
        /// Initializes a new instance of the <see cref="WebAPIExceptionFilter"/> class.
        /// </summary>
        /// <param name="hostEnvironment">The host environment.</param>
        /// <param name="logger">The logger instance.</param>
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
