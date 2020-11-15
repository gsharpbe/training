using System;
using System.Collections.Generic;
using System.Text;
using Metanous.Model.Core.Model;

namespace Training.Model
{
    public class Customer: ServiceModelBase
    {
        public string Name { get; set; }

        public List<Address> Addresses { get; set; }
        public List<Project> Projects { get; set; }

        public Customer()
        {
            Addresses = new List<Address>();
            Projects = new List<Project>();
        }
    }
}
