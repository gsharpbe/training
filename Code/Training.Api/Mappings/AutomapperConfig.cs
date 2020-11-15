using AutoMapper.Configuration;
using Training.Api.Mappings.Configuration;

namespace Training.Api.Mappings
{
    public class AutoMapperConfig : MapperConfigurationExpression
    {
        public AutoMapperConfig()
        {
            CustomerMapperConfig.CreateMappings(this);
            ProjectMapperConfig.CreateMappings(this);
        }
    }
}
