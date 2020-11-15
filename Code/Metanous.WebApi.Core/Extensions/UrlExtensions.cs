using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Metanous.WebApi.Core.Extensions
{
    public static class UrlExtensions
    {
        /// <summary>
        /// Combines two urls
        /// </summary>
        /// <param name="url1"></param>
        /// <param name="url2"></param>
        /// <returns></returns>
        public static string Combine(string url1, string url2)
        {
            return $"{url1.AppendSlashToPathIfNeeded()}{url2.RemoveSlashFromPathIfNeeded()}";
        }

        /// <summary>
        /// Serialize an object to form url encoded json (the same way jQuery.param())
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static IEnumerable<KeyValuePair<string, string>> SerializeToFormUrlEncodedJson(this object o)
        {
            var jObject = JObject.FromObject(o);

            var indices = new List<object>();

            foreach (var jProperty in jObject.Properties())
            {
                indices.Add(jProperty.Name);

                foreach (var s in Flatten(jProperty.Value, indices))
                {
                    yield return s;
                }

                indices.RemoveAt(indices.Count - 1);

                if (indices.Count != 0)
                    throw new InvalidOperationException("Something went wrong");
            }
        }

        /// <summary>
        /// Serialize an object to form url encoded json string (the same way jQuery.param())
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static string SerializeToFormUrlEncodedJsonString(this object o)
        {
            return string.Join("&", SerializeToFormUrlEncodedJson(o).Select(x => $"{x.Key}={x.Value}"));
        }

        private static string AppendSlashToPathIfNeeded(this string url)
        {
            if (url == null)
                return null;

            int length = url.Length;
            if (length == 0)
                return url;

            if (url[length - 1] != '/')
                url += '/';

            return url;
        }

        private static string RemoveSlashFromPathIfNeeded(this string url)
        {
            if (url == null)
                return null;

            int length = url.Length;
            if (length == 0)
                return url;

            if (url[0] == '/')
                url = url.Substring(1);

            return url;
        }

        private static IEnumerable<KeyValuePair<string, string>> Flatten(JToken jToken, List<object> indices)
        {
            if (jToken == null)
                yield break; // null values aren't serialized

            switch (jToken.Type)
            {
                case JTokenType.Array:
                    var jArray = (JArray)jToken;

                    for (int i = 0; i < jArray.Count; i++)
                    {
                        indices.Add(i);

                        foreach (var s in Flatten(jArray[i], indices))
                        {
                            yield return s;
                        }

                        indices.RemoveAt(indices.Count - 1);
                    }

                    break;
                case JTokenType.Object:
                    foreach (var jProperty in ((JObject)jToken).Properties())
                    {
                        indices.Add(jProperty.Name);

                        foreach (var s in Flatten(jProperty.Value, indices))
                        {
                            yield return s;
                        }

                        indices.RemoveAt(indices.Count - 1);
                    }

                    break;
                default:
                    var jValue = (JValue)jToken;

                    string value = jValue.Value == null ? "" : jValue.Value is string ? (string)jValue.Value : JsonConvert.SerializeObject(jValue.Value);
                    StringBuilder name = new StringBuilder();
                    for (int i = 0; i < indices.Count; i++)
                    {
                        var index = indices[i];

                        if (i > 0)
                            name.Append('[');

                        if (i < indices.Count - 1 || index is string)
                        {
                            // last array index not shown
                            name.Append(index);
                        }

                        if (i > 0)
                            name.Append(']');
                    }

                    yield return new KeyValuePair<string, string>(Uri.EscapeDataString(name.ToString()), Uri.EscapeDataString(value));
                    break;
            }
        }
    }
}