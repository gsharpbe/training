using Metanous.Model.Core.Model;

namespace Training.Model
{
    public class Project: ServiceModelBase
    {
        public string Name { get; set; }

        public Customer Customer { get; set; }
    }
}
