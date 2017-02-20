using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Text;
using Microsoft.AspNetCore.Diagnostics;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Custom_Errors_in_AspNetCore.Controllers
{
    public class ErrorController : Controller
    {
        // GET: /<controller>/
        private readonly ILogger<ErrorController> _logger;
        public ErrorController(ILogger<ErrorController> logger)
        {
            _logger = logger;
        }
        public IActionResult Index(int? id)
        {
            var logBuilder = new StringBuilder();

            var statusCodeReExecuteFeature = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();
            logBuilder.AppendLine($"Error {id} for {Request.Method} {statusCodeReExecuteFeature?.OriginalPath ?? Request.Path.Value}{Request.QueryString.Value}\n");

            var exceptionHandlerFeature = this.HttpContext.Features.Get<IExceptionHandlerFeature>();
            if (exceptionHandlerFeature?.Error != null)
            {
                var exception = exceptionHandlerFeature.Error;
                logBuilder.AppendLine($"<h1>Exception: {exception.Message}</h1>{exception.StackTrace}");
            }

            foreach (var header in Request.Headers)
            {
                var headerValues = string.Join(",", value: header.Value);
                logBuilder.AppendLine($"{header.Key}: {headerValues}");
            }
            _logger.LogError(logBuilder.ToString());

            if (id == null)
            {
                ViewBag.ErrorPage = "ExceptionError";
                return View();
            }

            switch (id.Value)
            {
                case 401:
                case 403:
                  //  ViewData["ErrorPage"]= "AccessDenied";
                    ViewBag.ErrorPage = "AccessDenied";
                    return View();
                case 404:
                    ViewBag.ErrorPage = "NotFound";
                    return View();

                default:
                    ViewBag.ErrorPage = "OtherErrors";
                    return View();
            }
        }
    }
}
