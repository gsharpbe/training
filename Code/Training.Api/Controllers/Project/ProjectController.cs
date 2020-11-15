using Microsoft.AspNetCore.Mvc;
using Training.Api.Controllers.Base;
using Training.Api.Services.Base;
using Training.Dal.Models;
using Training.Model;

namespace Training.Api.Controllers.Project
{
    [Route(Routes.Projects)]
    public class ProjectController: ModelControllerBase<Model.Project, ProjectModel>
    {
        public ProjectController(IService<Model.Project, ProjectModel> service) : base(service)
        {
        }
    }
}
