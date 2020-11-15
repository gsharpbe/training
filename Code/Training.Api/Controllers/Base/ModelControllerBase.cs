using System;
using System.Net;
using System.Threading.Tasks;
using Metanous.Model.Core.Model;
using Metanous.Model.Core.Search;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Training.Api.Services.Base;

namespace Training.Api.Controllers.Base
{
    public abstract class ModelControllerBase<TServiceModel, TModel> : ExtendedControllerBase
        where TServiceModel : ServiceModelBase, new()
        where TModel : ModelBase
    {
        protected readonly IService<TServiceModel, TModel> Service;

        protected ModelControllerBase(
            IService<TServiceModel, TModel> service)
        {
            Service = service;
            Service.ServiceContext = ServiceContext;
            Service.FilterExpressionBuilder.ServiceContext = ServiceContext;
            Service.SortExpressionBuilder.ServiceContext = ServiceContext;
        }

        [HttpGet]
        [Route("")]
        [Produces("application/json")]
        public virtual IActionResult Get(SearchParameters searchParameters)
        {
            if (ServiceContext.IfModifiedSince.HasValue)
            {
                searchParameters.IfModifiedSince = ServiceContext.IfModifiedSince;
            }
            var searchResult = Service.Find(searchParameters);

            if (searchResult == null)
                return NotFound();

            return Ok(searchResult);
        }

        [HttpGet]
        [Route("{id}")]
        [Produces("application/json")]
        public virtual IActionResult Get(Guid id)
        {
            var serviceModel = Service.GetByGuid(id);

            if (serviceModel == null)
                return NotFound();

            return Ok(serviceModel);
        }

        [HttpPost]
        [Route("")]
        [Produces("application/json")]
        public virtual Task<IActionResult> Create([FromBody]TServiceModel serviceModel)
        {
            if (serviceModel == null)
                return Task.FromResult((IActionResult)BadRequest());

            var guid = Service.Add(serviceModel);

            var baseUri = new Uri(string.Concat(Request.Scheme, "://", Request.Host.Value, Request.Path, "/"));

            var location = new Uri(baseUri, guid.ToString());

            return Task.FromResult((IActionResult)Created(location, new TServiceModel { Guid = guid }));
        }

        [HttpPatch]
        [Route("{id}")]
        [Produces("application/json")]
        public virtual Task<IActionResult> Patch(Guid id, [FromBody] JsonPatchDocument<TServiceModel> patch)
        {
            Service.Patch(id, patch);

            return Task.FromResult((IActionResult)Ok());
        }

        [HttpPut]
        public virtual Task<IActionResult> Update([FromBody]TServiceModel serviceModel)
        {
            if (serviceModel == null)
                return Task.FromResult((IActionResult)BadRequest());

            Service.Update(serviceModel);

            return Task.FromResult((IActionResult)Ok(new {serviceModel.Guid }));
        }

        [HttpDelete]
        [Route("{id}")]
        public virtual Task<IActionResult> Delete(Guid id)
        {
            Service.Delete(id.ToString());

            return Task.FromResult((IActionResult)NoContent());
        }

        [HttpPost]
        [Route("commands")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ICommandResult), (int)HttpStatusCode.OK)]
        public virtual Task<IActionResult> Execute([FromBody] CommandBase command)
        {
            if (command == null)
                return Task.FromResult((IActionResult)BadRequest());

            var result = Service.Execute(command);

            return Task.FromResult((IActionResult)Ok(result));
        }
    }
}
