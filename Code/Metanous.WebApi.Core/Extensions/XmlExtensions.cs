using System;
using System.IO;
using System.Xml.Serialization;

namespace Metanous.WebApi.Core.Extensions
{
    public static class XmlExtensions
    {
        public static string ToXml(this object obj)
        {
            try
            {
                var serializer = new XmlSerializer(obj.GetType());
                using (var writer = new StringWriter())
                {
                    serializer.Serialize(writer, obj);
                    return writer.ToString();
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static T FromXml<T>(this string xml)
        {
            var serializer = new XmlSerializer(typeof(T));
            try
            {
                using (var reader = new StringReader(xml))
                {
                    object obj = serializer.Deserialize(reader);
                    return (T)obj;
                }
            }
            catch (Exception)
            {
                return default(T);
            }
        }
    }
}