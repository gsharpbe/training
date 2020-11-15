using Metanous.Model.Core.Model;

namespace Training.Dal.Models
{
    public class AddressModel : ModelBase
    {
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string Zip { get; set; }
        public string City { get; set; }

        public virtual CustomerModel Customer { get; set; }
    }
}