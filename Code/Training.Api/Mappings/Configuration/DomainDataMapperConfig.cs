using AutoMapper;
using Metanous.WebApi.Core.Extensions;
using Training.Dal.Models;
using Training.Model;

namespace Training.Api.Mappings.Configuration
{
    public class DomainDataMapperConfig
    {
        public static void CreateMappings(IMapperConfigurationExpression config)
        {
            CreateCountryMappings(config);
        }

        private static void CreateCountryMappings(IMapperConfigurationExpression config)
        {
            config.CreateServiceModelMap<CountryModel, Country>();
            config.CreateModelMap<Country, CountryModel>();
        }
    }
}
