using AutoMapper;
using Microsoft.Extensions.Logging;
using Training.Api.Services.Base;
using Training.Dal.Context;
using Training.Dal.Models;

namespace Training.Api.Services.Customer
{
    public class CustomerService: Service<Model.Customer, CustomerModel>
    {
        public CustomerService(DataContext dataContext, IMapper mapper, ILogger<CustomerService> logger) : base(dataContext, mapper, logger)
        {
        }
    }
}
