using Metanous.Model.Core.Model;

namespace Training.Model
{
    public class Address: ServiceModelBase
    {
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string Zip { get; set; }
        public string City { get; set; }
    }
}
