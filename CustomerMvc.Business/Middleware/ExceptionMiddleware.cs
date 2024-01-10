using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;

namespace CustomerMvc.Business.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception: {ex.Message}");
                _logger.LogError($"Inner Exception: {ex.InnerException?.Message}");
                _logger.LogError($"Stack Trace: {ex.StackTrace}");

                context.Items["StatusCode"] = HttpStatusCode.InternalServerError;
                context.Items["Message"] = ex.Message;
                context.Response.Redirect($"/Home/Error");
            }
        }
    }
}
