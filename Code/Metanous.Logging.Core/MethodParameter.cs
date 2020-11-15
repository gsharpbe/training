using System.Text;

namespace Metanous.Logging.Core
{
    public class MethodParameter
    {
        public string Name { get; set; }
        public object Value { get; set; }

        public MethodParameter(string name, object value)
        {
            Name = name;
            Value = value;
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.Append('[');
            builder.Append(Name);
            builder.Append("]=");
            builder.Append("{@");
            builder.Append(Name);
            builder.Append("}");
            return builder.ToString();
        }
    }
}
