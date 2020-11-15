using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Metanous.Model.Core.Extensions
{
    public static class StringExtensions
    {
        public static string NormalizeName(this string text)
        {
            return text.Replace("\"", "");
        }

        public static bool Matches(this string text, string pattern)
        {
            return Regex.IsMatch(text, pattern);
        }

        /// <summary>
        /// Removes the suffix from the string, if it exists.
        /// </summary>
        /// <param name="s">The original string.</param>
        /// <param name="suffix">The suffix string.</param>
        /// <returns>The original string with the suffix string removed, if it's present in the original string.</returns>
        public static string RemoveSuffix(this string s, string suffix)
        {
            if (s.EndsWith(suffix))
            {
                return s.Substring(0, s.Length - suffix.Length);
            }
            return s;
        }

        /// <summary>
        /// Iterates through the 'suffixes' dictionary and replaces the suffix from the string with another string.
        /// After the first found suffix, it replaces that suffix and exits.
        /// </summary>
        /// <param name="s">The original string.</param>
        /// <param name="suffixes">The dictionary of key value pairs (key = search suffix, value = replacement string).</param>
        /// <returns>The original string with the first-found search suffix string replaced with the according replacement string.</returns>
        public static string ReplaceSuffix(this string s, IDictionary<string, string> suffixes)
        {
            if (suffixes != null)
            {
                foreach (KeyValuePair<string, string> kv in suffixes)
                {
                    if (s.EndsWith(kv.Key))
                    {
                        return s.Substring(0, s.Length - kv.Key.Length) + kv.Value;
                    }
                }
            }
            return s;
        }

        public static string ReplaceSuffix(this string s, string search, string replace)
        {
            return ReplaceSuffix(s, new Dictionary<string, string> { { search, replace } });
        }

        /// <summary>
        /// Removes the prefix from the string, if it exists.
        /// </summary>
        /// <param name="s">The original string.</param>
        /// <param name="prefix">The prefix string.</param>
        /// <returns>The original string with the prefix string removed, if it's present in the original string.</returns>
        public static string RemovePrefix(this string s, string prefix)
        {
            if (s.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            {
                return s.Substring(prefix.Length);
            }
            return s;
        }

        public static IEnumerable<Guid> ToGuids(this string ids)
        {
            var guids = new List<Guid>();
            if (ids == null)
            {
                return guids;
            }

            ids.Split(',').ForEach(id =>
            {
                if (Guid.TryParse(id, out var guid))
                {
                    guids.Add(guid);
                }
            });

            return guids;
        }

        public static IEnumerable<T> ToEnums<T>(this string ids)
        {
            var enums = new List<T>();

            ids.Split(',').ForEach(id =>
            {
                var enumValue = (T)Enum.Parse(typeof(T), id);
                enums.Add(enumValue);
            });

            return enums;
        }

        public static IEnumerable<Enum> ToEnums(this string ids, Type enumType)
        {
            var enums = new List<Enum>();

            ids.Split(',').ForEach(id =>
            {
                var enumValue = (Enum)Enum.Parse(enumType, id);
                enums.Add(enumValue);
            });

            return enums;
        }

        public static string FirstToLower(this string value)
        {
            return char.ToLowerInvariant(value[0]) + value.Substring(1);
        }

        public static string FirstToUpper(this string value)
        {
            return char.ToUpperInvariant(value[0]) + value.Substring(1);
        }

        public static string OnlyLettersOrDigits(this string value)
        {
            if (value == null)
                return null;

            var result = string.Empty;

            foreach (var c in value)
            {
                if (char.IsLetterOrDigit(c))
                {
                    result += c;
                }
            }

            return result;
        }

    }
}
