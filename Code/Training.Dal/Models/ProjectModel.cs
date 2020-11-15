using Metanous.Model.Core.Model;

namespace Training.Dal.Models
{
    public class ProjectModel : ModelBase
    {
        public string Name { get; set; }

        public virtual CustomerModel Customer { get; set; }
    }
}