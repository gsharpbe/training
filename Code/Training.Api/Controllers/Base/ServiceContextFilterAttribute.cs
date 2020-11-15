using System;
using System.Linq;
using Metanous.Model.Core.Search;
using Metanous.WebApi.Core.Extensions;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Training.Api.Controllers.Base
{
    public class ServiceContextFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext actionExecutingContext)
        {
            // check if the filter is applied on controllers inheriting from BaseApiController
            if (!(actionExecutingContext.Controller is ExtendedControllerBase controller)) return;

            var includes = new Includes();

            // retrieve select keys from header and store in service context
            var includeHeaders = actionExecutingContext.HttpContext.FromHeadersWithPrefix(Includes.IncludePrefix);
            foreach (var includeHeader in includeHeaders)
            {
                if (includeHeader.Key.ToLower() == "all")
                {
                    if (bool.TryParse(includeHeader.Value.FirstOrDefault(), out var includeAll))
                    {
                        includes.IncludeAll = includeAll;
                    }
                }
                else
                {
                    includes.Add(includeHeader.Key, includeHeader.Value);
                }
            }

            controller.ServiceContext.Includes = includes;

            // application
            controller.ServiceContext.IsApplication = actionExecutingContext.HttpContext.IsApplication();
            controller.ServiceContext.ApplicationId = actionExecutingContext.HttpContext.GetApplicationId();

            // username 
            controller.ServiceContext.UserName = actionExecutingContext.HttpContext.GetUserId();

            // entity id
            controller.ServiceContext.EntityId = actionExecutingContext.HttpContext.GetEntityId();

            // content language
            var contentLanguage = actionExecutingContext.HttpContext.FromHeaders("Content-Language");
            if (!string.IsNullOrWhiteSpace(contentLanguage))
            {
                controller.ServiceContext.ContentLanguage = contentLanguage.ToLower();
            }

            // if-modified-since
            var ifModifiedSince = actionExecutingContext.HttpContext.FromHeaders("if-modified-since");
            if (!string.IsNullOrWhiteSpace(ifModifiedSince))
            {
                if (DateTimeOffset.TryParse(ifModifiedSince, out var date))
                {
                    controller.ServiceContext.IfModifiedSince = date;
                }
            }
        }
    }
}
