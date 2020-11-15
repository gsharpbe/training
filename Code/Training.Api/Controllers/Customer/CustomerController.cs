using Microsoft.AspNetCore.Mvc;
using Training.Api.Controllers.Base;
using Training.Api.Services.Base;
using Training.Dal.Models;
using Training.Model;

namespace Training.Api.Controllers.Customer
{
    [Route(Routes.Customers)]
    public class CustomerController: ModelControllerBase<Model.Customer, CustomerModel>
    {
        public CustomerController(IService<Model.Customer, CustomerModel> service) : base(service)
        {
        }
    }
}
