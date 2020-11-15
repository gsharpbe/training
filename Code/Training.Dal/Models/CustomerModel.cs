using System.Collections.Generic;
using Metanous.Model.Core.Model;

namespace Training.Dal.Models
{
    public class CustomerModel : ModelBase
    {
        public string Name { get; set; }

        public virtual IList<AddressModel> Addresses { get; set; }
        public virtual IList<ProjectModel> Projects { get; set; }

        public CustomerModel()
        {
            Addresses = new List<AddressModel>();
            Projects = new List<ProjectModel>();
        }
    }
}