using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace ServianOps_Backend.Middlewares
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
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
                _logger.LogError(ex, "An unhandled exception occurred during the request.");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            
            // By default, assume 500 Internal Server Error
            var statusCode = (int)HttpStatusCode.InternalServerError;
            var message = "An internal server error occurred.";

            // We can map specific exceptions to different status codes.
            // For example, if it's a known validation exception or business exception from the Service layer:
            if (exception is ArgumentException || exception.Message.Contains("already registered") || exception.Message.Contains("already taken"))
            {
                statusCode = (int)HttpStatusCode.BadRequest;
                message = exception.Message;
            }
            
            context.Response.StatusCode = statusCode;

            var result = JsonSerializer.Serialize(new
            {
                error = message,
                statusCode = statusCode
            });

            return context.Response.WriteAsync(result);
        }
    }
}
