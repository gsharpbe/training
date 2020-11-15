using System;
using System.Net;

namespace Metanous.WebApi.Core.Http
{
    public class ApiException : Exception
    {
        public HttpStatusCode HttpStatusCode { get; set; } = HttpStatusCode.InternalServerError;

        public ApiException() { }

        public ApiException(string message) : base(message) { }

        public ApiException(string message, Exception innerException) : base(message, innerException) { }

        public ApiException(HttpStatusCode httpStatusCode, string message = "") : base(message)
        {
            HttpStatusCode = httpStatusCode;
        }
    }
}