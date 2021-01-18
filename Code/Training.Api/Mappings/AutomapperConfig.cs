using AutoMapper.Configuration;
using Training.Api.Mappings.Configuration;

namespace Training.Api.Mappings
{
    public class AutoMapperConfig : MapperConfigurationExpression
    {
        public AutoMapperConfig()
        {
            DomainDataMapperConfig.CreateMappings(this);
            CustomerMapperConfig.CreateMappings(this);
            ProjectMapperConfig.CreateMappings(this);
        }
    }
}
