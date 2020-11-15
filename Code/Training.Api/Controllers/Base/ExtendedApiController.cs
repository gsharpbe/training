using System;
using System.Linq;
using System.Net;
using Metanous.WebApi.Core.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Training.Api.Controllers.Base
{
    [Authorize]
    [ServiceFilter(typeof(ServiceContextFilterAttribute))]
    public abstract class ExtendedControllerBase : ControllerBase
    {
        public ServiceContext ServiceContext { get; set; } = new ServiceContext();

        protected IActionResult NotModified()
        {
            return StatusCode((int)HttpStatusCode.NotModified);
        }

        protected internal virtual OkWithETagResult Ok(object content, string eTag, DateTime? lastModified = null)
        {
            return new OkWithETagResult(content) { ETagValue = eTag, LastModified = lastModified };
        }

        protected IActionResult OkIfNotModified<T>(T content, DateTime? lastModified = null, JsonSerializerSettings settings = null)
        {
            var eTag = content.GenerateETag(settings);

            return Request.Headers.Any(x => Equals(x.Value.ToString(), eTag)) ? NotModified() : Ok(content, eTag, lastModified);
        }
    }
}