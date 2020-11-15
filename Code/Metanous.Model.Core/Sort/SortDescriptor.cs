using Newtonsoft.Json;

namespace Metanous.Model.Core.Sort
{
    public class SortDescriptor
    {
        public SortDirection Direction { get; set; }
        public string Field { get; set; }
    }
}