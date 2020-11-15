using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Metanous.WebApi.Core.Extensions;
using Training.Dal.Models;
using Training.Model;

namespace Training.Api.Mappings.Configuration
{
    public static class ProjectMapperConfig
    {
        public static void CreateMappings(IMapperConfigurationExpression config)
        {
            CreateProjectMappings(config);
        }

        private static void CreateProjectMappings(IMapperConfigurationExpression config)
        {
            config.CreateServiceModelMap<ProjectModel, Project>();
            config.CreateModelMap<Project, ProjectModel>()
                .MapReference(x => x.Customer, x => x.Customer);
        }

    }
}
