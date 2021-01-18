using Microsoft.AspNetCore.Mvc;
using Training.Api.Controllers.Base;
using Training.Api.Services.Base;
using Training.Dal.Models;
using Training.Model;

namespace Training.Api.Controllers.DomainData
{
    [Route(Routes.Countries)]
    public class CountryController: ModelControllerBase<Country, CountryModel>
    {
        public CountryController(IService<Country, CountryModel> service) : base(service)
        {
        }
    }
}
