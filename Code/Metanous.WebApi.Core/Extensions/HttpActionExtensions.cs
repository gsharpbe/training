using System;
using System.Collections.Generic;
using System.Linq;
using Metanous.Model.Core.Extensions;
using Microsoft.AspNetCore.Http;

namespace Metanous.WebApi.Core.Extensions
{
    public static class HttpActionExtensions
    {
        public static string FromQueryString(this HttpContext actionContext, string key, string defaultValue = null)
        {
            var value = actionContext.Request.GetQueryString(key);
            return (string.IsNullOrEmpty(value) && !string.IsNullOrEmpty(defaultValue)) ? defaultValue : value;
        }

        public static string FromHeaders(this HttpContext actionContext, string key, string defaultValue = null)
        {
            var value = actionContext.Request.GetHeader(key);
            return (string.IsNullOrEmpty(value) && !string.IsNullOrEmpty(defaultValue)) ? defaultValue : value;
        }

        public static string FromAny(this HttpContext actionContext, string key, string defaultValue = null)
        {
            var value = actionContext.FromQueryString(key);
            if (string.IsNullOrEmpty(value))
            {
                value = actionContext.FromHeaders(key);
            }
            return (string.IsNullOrEmpty(value) && !string.IsNullOrEmpty(defaultValue)) ? defaultValue : value;
        }

        public static IEnumerable<KeyValuePair<string, IReadOnlyCollection<string>>> FromHeadersWithPrefix(this HttpContext actionContext, string keyPrefix)
        {
            return actionContext.Request.Headers
                .Where(header => header.Key.StartsWith(keyPrefix, StringComparison.OrdinalIgnoreCase))
                .Select(
                    header =>
                        new KeyValuePair<string, IReadOnlyCollection<string>>(header.Key.RemovePrefix(keyPrefix), header.Value.First().Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(e => e.Trim()).ToList()));
        }
    }
}