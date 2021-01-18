using AutoMapper;
using Microsoft.Extensions.Logging;
using Training.Api.Services.Base;
using Training.Dal.Context;
using Training.Dal.Models;
using Training.Model;

namespace Training.Api.Services.DomainData
{
    public class CountryService: Service<Country, CountryModel>
    {
        public CountryService(DataContext dataContext, IMapper mapper, ILogger<CountryService> logger) : base(dataContext, mapper, logger)
        {
        }
    }
}
