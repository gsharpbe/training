using System;
using System.Net;
using System.Threading.Tasks;
using Metanous.WebApi.Core.Http;
using Microsoft.AspNetCore.Http;
using Serilog;
using Training.Configuration;

namespace Training.Api.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        private static readonly ILogger Log = Serilog.Log.ForContext<ExceptionMiddleware>();

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                context.Response.ContentType = "text/plain";
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                var message = $"{ex.Message}\n\n{ex.StackTrace}";

                if (ex is ApiException apiException)
                {
                    context.Response.StatusCode = (int)apiException.HttpStatusCode;
                    message = ex.Message;
                }
                else
                {
                    Log.Error(message);

                    if (Settings.Current.BuildConfiguration == BuildConfiguration.Production)
                    {
                        message = "An error occurred, please try again or contact the administrator.";
                    }
                }

                await context.Response.WriteAsync(message);
            }
        }
    }
}
