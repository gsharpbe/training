using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Metanous.Model.Core.Extensions
{
    public static class EnumExtensions
    {
        /// <summary>
        /// Searches for a localized description in the given resource manager for an enum.
        /// 
        /// </summary>
        /// <param name="obj">the enum value</param>
        /// <param name="resourceManager">ResourceManager to search</param>
        /// <param name="cultureInfo"></param>
        /// <param name="format">format of the resource key, defaults to "{0}_{1}" where {0} is the enum class name and {1} the enum value name</param>
        /// <returns>The localized description or null when not found</returns>
        public static string GetEnumDescription(this object obj, ResourceManager resourceManager, CultureInfo cultureInfo = null, string format = "{0}_{1}")
        {
            var type = obj.GetType();
            var typeInfo = type.GetTypeInfo();

            if (!typeInfo.IsEnum)
            {
                return null;
            }
            try
            {
                var enumName = typeInfo.Name;
                var enumValueName = Enum.GetName(type, obj);

                if (cultureInfo == null)
                    return resourceManager.GetString(string.Format(format, enumName, enumValueName)) ?? enumValueName;

                return resourceManager.GetString(string.Format(format, enumName, enumValueName), cultureInfo) ?? enumValueName;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}