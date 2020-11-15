using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using CommandBase = Metanous.Model.Core.Model.CommandBase;

namespace Metanous.Model.Core.Converters
{
    public class CommandJsonConverter : JsonConverter
    {
        private static readonly List<Type> CommandTypes;

        static CommandJsonConverter()
        {
            CommandTypes =
                AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(x => x.GetTypes())
                    .Where(x => typeof(CommandBase).GetTypeInfo().IsAssignableFrom(x))
                    .ToList();
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(CommandBase).GetTypeInfo().IsAssignableFrom(objectType.GetTypeInfo());
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jObject = JObject.Load(reader);

            var type = jObject["command"]?.Value<string>() ?? jObject["Command"]?.Value<string>();

            var commandBase = CreateInstance(type);
            serializer.Populate(jObject.CreateReader(), commandBase);
            return commandBase;
        }

        private CommandBase CreateInstance(string typeName)
        {
            var type = CommandTypes.FirstOrDefault(it => string.Equals(it.Name, typeName, StringComparison.OrdinalIgnoreCase));
            if (type == null) throw new ArgumentException($"Unknown command: {typeName ?? "null"}.");
            
            var result = Activator.CreateInstance(type);
            return result as CommandBase;
        }

        /// <summary>
        /// When persisting as JSON, we need to write the type of the derived class to be able to deserialize correctly
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        /// <param name="serializer"></param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var type = value.GetType().Name;

            // We want to use the default serializer but add an extra property.
            // We cannot invoke the default serializer on the 'value' object, because that would recursively
            // invoke this method again, leading into a stackoverflow.

            // This seems to be a design flaw in JSON.net.
            // We work around it by copying our object into a dynamic object and serializing that object.
            IDictionary<string, object> expando = new ExpandoObject();

            foreach (var property in value.GetType().GetRuntimeProperties())
            {
                if (property.GetCustomAttribute<JsonIgnoreAttribute>() != null)
                {
                    continue;
                }

                var propertyValue = property.GetValue(value);

                var jsonAttribute = property.GetCustomAttribute<JsonPropertyAttribute>();
                if (jsonAttribute == null)
                {
                    // Default case - just copy the property
                    expando.Add(property.Name, propertyValue);
                }
                else
                {
                    // We need special handling
                    if (jsonAttribute.NullValueHandling == NullValueHandling.Ignore && propertyValue == null)
                    {
                        continue;
                    }
                    expando.Add(property.Name, propertyValue);
                }
            }

            var jo = JObject.FromObject(expando, serializer);
            if (!string.IsNullOrEmpty(type))
            {
                jo.Add("command", type);
            }
            jo.WriteTo(writer);
        }
    }
}