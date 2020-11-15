using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Metanous.WebApi.Core.Extensions
{
    public static class HttpContextExtensions
    {
        public static bool IsApplication(this HttpContext httpContext)
        {
            return httpContext.User.IsApplication();
        }

        public static string GetApplicationId(this HttpContext httpContext)
        {
            return httpContext.User.GetApplicationId();
        }

        public static string GetUserId(this HttpContext httpContext)
        {
            return httpContext.User.GetUserId();
        }

        public static string GetEntityId(this HttpContext httpContext)
        {
            return httpContext.User.GetEntityId();
        }

        public static bool IsApplication(this ClaimsPrincipal principal)
        {
            return principal.FindFirst("sub") == null;
        }

        public static string GetApplicationId(this ClaimsPrincipal principal)
        {
            return principal.FindFirst("client_id")?.Value;
        }

        public static string GetUserId(this ClaimsPrincipal principal)
        {
            return principal.Identity.Name;
        }

        public static string GetEntityId(this ClaimsPrincipal principal)
        {
            return principal.IsApplication() ? principal.GetApplicationId() : principal.GetUserId();
        }
    }
}
