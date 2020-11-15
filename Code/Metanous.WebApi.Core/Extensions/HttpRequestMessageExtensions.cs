using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;

namespace Metanous.WebApi.Core.Extensions
{
    public static class HttpRequestMessageExtensions
    {
        public static Uri RequestUri(this HttpRequest request)
        {
            return new Uri(request.GetDisplayUrl());
        }

        /// <summary>
        /// Reads HTML form URL encoded data provided in the URI query string as an object of a specified type.
        /// </summary>
        /// 
        /// <returns>
        /// true if the query component of the URI can be read as the specified type; otherwise, false.
        /// </returns>
        /// <typeparam name="T">The type of object to read.</typeparam>
        public static T ReadQueryAs<T>(this HttpRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            var formUrlEncodedJson = request.Query.ToDictionary(x => x.Key, x => string.Join(",", x.Value));
            var contentJson = JsonConvert.SerializeObject(formUrlEncodedJson);
            return JsonConvert.DeserializeObject<T>(contentJson);
        }

        public static IEnumerable<KeyValuePair<string, string>> GetHeaderNameValuePairs(this HttpRequest request)
        {
            return request.Headers.Select(
                header => new KeyValuePair<string, string>(header.Key, string.Join(" ", header.Value)));
        }

        public static IEnumerable<KeyValuePair<string, string>> GetQueryNameValuePairs(this HttpRequest request)
        {
            if (request == null) return null;

            var uri = request.RequestUri();

            if (string.IsNullOrEmpty(uri?.Query))
            {
                return Enumerable.Empty<KeyValuePair<string, string>>();
            }

            // https://stackoverflow.com/questions/29992848/parse-and-modify-a-query-string-in-net-core
            var pairs = QueryHelpers.ParseQuery(uri.Query);
            return pairs.Select(pair => new KeyValuePair<string, string>(pair.Key, string.Join(" ", pair.Value)));
        }

        public static string GetQueryString(this HttpRequest request, string key, string defaultValue = null)
        {
            var queryStrings = request.Query;
            if (queryStrings == null) return defaultValue;

            var match = queryStrings.FirstOrDefault(kv => kv.Key.EqualsIgnoreCase(key));
            return string.IsNullOrEmpty(match.Value) ? defaultValue : match.Value.ToString();
        }

        public static string GetHeader(this HttpRequest request, string key, string defaultValue = null)
        {
            var kvPair = request.Headers.FirstOrDefault(h => h.Key.EqualsIgnoreCase(key));
            return string.IsNullOrWhiteSpace(kvPair.Value) ? defaultValue : kvPair.Value.FirstOrDefault();
        }

        public static Uri GetApplicationBaseUri(this HttpRequest request)
        {
            var baseUrl = request.RequestUri().GetLeftPart(UriPartial.Authority);

            return new Uri(baseUrl);
        }

        private static bool EqualsIgnoreCase(this string s, string compareWith)
        {
            return s.Equals(compareWith, StringComparison.OrdinalIgnoreCase);
        }
    }
}