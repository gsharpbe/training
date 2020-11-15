using System;
using System.Collections.Generic;
using Metanous.Model.Core.Model;
using Metanous.Model.Core.Search;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Training.Api.Controllers.Base;
using Training.Api.Services.Background;
using Training.Api.Services.Expressions;

namespace Training.Api.Services.Base
{
    public interface IService<TServiceModel, TModel>
        where TServiceModel : ServiceModelBase
        where TModel : ModelBase
    {
        ServiceContext ServiceContext { get; set; }

        List<TServiceModel> GetAll(SearchParameters searchParameters = null);

        SearchResult<TServiceModel> Find(SearchParameters searchParameters);

        TServiceModel GetByGuid(Guid guid);

        TModel GetModelByGuid(Guid guid);

        Guid Add(TServiceModel serviceModel);

        void Patch(Guid id, [FromBody] JsonPatchDocument<TServiceModel> patch);

        void Delete(string id);

        void Update(TServiceModel serviceModel);

        ICommandResult Execute(CommandBase command);

        FilterExpressionBuilder<TModel> FilterExpressionBuilder { get; set; }
        SortExpressionBuilder<TModel> SortExpressionBuilder { get; set; }

        BackgroundTaskStatus ReadBackgroundTaskStatus(Guid id);
        
    }
}
