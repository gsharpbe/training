using Metanous.Model.Core.Model;

namespace Training.Dal.Models
{
    public class CountryModel: ModelBase
    {
        public string IsoCode { get; set; }
        public string Name { get; set; }
    }
}
