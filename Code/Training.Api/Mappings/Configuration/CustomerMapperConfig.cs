using AutoMapper;
using Metanous.WebApi.Core.Extensions;
using Training.Dal.Models;
using Training.Model;

namespace Training.Api.Mappings.Configuration
{
    public class CustomerMapperConfig
    {
        public static void CreateMappings(IMapperConfigurationExpression config)
        {
            CreateCustomerMappings(config);
            CreateAddressMappings(config);
        }

        private static void CreateCustomerMappings(IMapperConfigurationExpression config)
        {
            config.CreateServiceModelMap<CustomerModel, Customer>();
            config.CreateModelMap<Customer, CustomerModel>();
        }

        private static void CreateAddressMappings(IMapperConfigurationExpression config)
        {
            config.CreateServiceModelMap<AddressModel, Address>();
            config.CreateModelMap<Address, AddressModel>();
        }
    }
}
