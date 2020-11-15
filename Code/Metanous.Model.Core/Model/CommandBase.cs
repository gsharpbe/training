using Metanous.Model.Core.Converters;
using Newtonsoft.Json;

namespace Metanous.Model.Core.Model
{
    [JsonConverter(typeof(CommandJsonConverter))]
    public class CommandBase
    {
    }
}
