using AutoMapper;
using Microsoft.Extensions.Logging;
using Training.Api.Services.Base;
using Training.Dal.Context;
using Training.Dal.Models;

namespace Training.Api.Services.Project
{
    public class ProjectService: Service<Model.Project, ProjectModel>
    {
        public ProjectService(DataContext dataContext, IMapper mapper, ILogger<ProjectService> logger) : base(dataContext, mapper, logger)
        {
        }
    }
}
