using System.Net;
using System.Text.Json;

namespace WorkflowTrackingSystem.API.Middlewares
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
                _logger.LogError(ex, "Unhandled exception occurred");

                var statusCode = GetStatusCode(ex);

                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)statusCode;

                var response = new
                {
                    success = false,
                    message = ex.Message,
                    details = ex.InnerException?.Message,
                    statusCode = (int)statusCode
                };

                var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
                var json = JsonSerializer.Serialize(response, options);

                await context.Response.WriteAsync(json);
            }
        }

        private static HttpStatusCode GetStatusCode(Exception ex)
        {
            return ex switch
            {
                KeyNotFoundException => HttpStatusCode.NotFound,             // 404
                InvalidOperationException => HttpStatusCode.BadRequest,      // 400
                ArgumentException => HttpStatusCode.BadRequest,              // 400
                _ => HttpStatusCode.InternalServerError                      // 500
            };
        }
    }
}
