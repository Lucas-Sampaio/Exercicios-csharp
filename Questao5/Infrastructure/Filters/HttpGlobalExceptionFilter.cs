using Microsoft.AspNetCore.Mvc;
using System.Net;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Questao5.Infrastructure.Filters
{
    public class HttpGlobalExceptionFilter(IWebHostEnvironment env, ILogger<HttpGlobalExceptionFilter> logger) : IExceptionFilter
    {
        private readonly IWebHostEnvironment _env = env;
        private readonly ILogger<HttpGlobalExceptionFilter> _logger = logger;

        public void OnException(ExceptionContext context)
        {
            _logger.LogError(new EventId(context.Exception.HResult),
                     context.Exception,
                     context.Exception.Message);

            var json = new JsonErrorResponse
            {
                Messages = ["Ocorreu um erro inesperado tente novamente."]
            };

            if (_env.IsDevelopment())
            {
                json.DeveloperMessage = new DeveloperErrorMessage
                {
                    Message = context.Exception.Message,
                    Stacktrace = context.Exception.StackTrace,
                    InnerExceptionMessage = context.Exception.InnerException?.Message
                };
            }

            context.Result = new BadRequestObjectResult(json);
            context.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        }

        private class JsonErrorResponse
        {
            public string[]? Messages { get; set; }

            public DeveloperErrorMessage? DeveloperMessage { get; set; }
        }

        private class DeveloperErrorMessage
        {
            public string? Message { get; set; }
            public string? Stacktrace { get; set; }
            public string? InnerExceptionMessage { get; set; }
        }
    }
}
