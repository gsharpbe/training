using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Metanous.Logging.Core;
using Metanous.Model.Core.Extensions;
using Metanous.Model.Core.Filter;
using Metanous.Model.Core.Model;
using Metanous.Model.Core.Search;
using Metanous.WebApi.Core.Extensions;
using Metanous.WebApi.Core.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Training.Api.Controllers.Base;
using Training.Api.Services.Background;
using Training.Api.Services.Expressions;
using Training.Configuration;
using Training.Dal.Context;

namespace Training.Api.Services.Base
{
    public abstract class Service<TServiceModel, TModel> : IService<TServiceModel, TModel>
       where TModel : ModelBase, new()
       where TServiceModel : ServiceModelBase
    {
        protected readonly IBackgroundTaskService BackgroundTaskService;
        public SortExpressionBuilder<TModel> SortExpressionBuilder { get; set; }
        public FilterExpressionBuilder<TModel> FilterExpressionBuilder { get; set; }

        public DataContext DataContext { get; }

        private ServiceContext _serviceContext;
        public ServiceContext ServiceContext
        {
            get => _serviceContext;
            set
            {
                _serviceContext = value;
                if (FilterExpressionBuilder != null)
                {
                    FilterExpressionBuilder.ServiceContext = _serviceContext;
                }
            }
        }

        protected DbSet<TModel> DbSet { get; }
        protected IMapper Mapper { get; }
        protected ILogger Logger { get; }

        protected bool SilentlyIgnoreDuplicates { get; set; }

        protected Service(DataContext dataContext, IMapper mapper, ILogger logger)
        {
            FilterExpressionBuilder = new FilterExpressionBuilder<TModel>();
            SortExpressionBuilder = new SortExpressionBuilder<TModel>();

            Logger = logger;
            Mapper = mapper;

            DataContext = dataContext;
            DbSet = dataContext.Set<TModel>();
        }

        protected Service(DataContext dataContext, IMapper mapper, ILogger logger, IBackgroundTaskService backgroundTaskService): this(dataContext, mapper, logger)
        {
            BackgroundTaskService = backgroundTaskService;
        }

        #region api

        public IQueryable<TModel> GetAllModels(SearchParameters searchParameters)
        {
            var query = DbSet.AsQueryable();

            query = query.Where(x => !x.IsObsolete);

            query = ApplyIfModifiedSince(query, searchParameters);

            query = ApplySearch(query, searchParameters);

            query = FilterExpressionBuilder.ApplyFiltering(query, searchParameters);

            query = SortExpressionBuilder.ApplySorting(query, searchParameters);

            return query;
        }

        public List<TServiceModel> GetAll(SearchParameters searchParameters = null)
        {
            using (Logger.LogMethodEnter(new[] { new MethodParameter("SearchParameters", searchParameters) }))
            {
                var models = GetAllModels(searchParameters).ToList();
                var items = models.Select(Map).ToList();

                return items;
            }
        }

        public SearchResult<TServiceModel> Find(SearchParameters searchParameters = null)
        {
            using (Logger.LogMethodEnter(new[] { new MethodParameter("SearchParameters", searchParameters) }))
            {
                DataContext.ChangeTracker.AutoDetectChangesEnabled = false;

                var query = GetAllModels(searchParameters);

                var searchResult = new SearchResult<TServiceModel>();

                if (searchParameters == null || searchParameters.SkipCount == false)
                {
                    searchResult.Total = query.Count();
                }

                if (searchParameters == null || searchParameters.SkipData == false)
                {
                    query = ApplyEntityFrameworkIncludes(query, true);

                    query = ApplyPaging(query, out int startIndex, searchParameters);

                    var models = query.ToList();

                    var items = models.Select(Map).ToList();

                    searchResult.Data = items;
                    searchResult.ItemsCount = items.Count;
                    searchResult.StartIndex = startIndex;
                }

                AfterFind(searchResult);

                return searchResult;
            }
        }

        protected virtual void AfterFind(SearchResult<TServiceModel> searchResult)
        {
        }

        public TServiceModel GetById(string id)
        {
            DataContext.ChangeTracker.AutoDetectChangesEnabled = false;

            using (Logger.LogMethodEnter(new[] { new MethodParameter("Id", id) }))
            {
                var model = GetModelById(id);
                var serviceModel = Map(model);

                AfterGet(serviceModel);

                return serviceModel;
            }
        }

        public TServiceModel GetByGuid(Guid guid)
        {
            DataContext.ChangeTracker.AutoDetectChangesEnabled = false;

            using (Logger.LogMethodEnter(new[] { new MethodParameter("Guid", guid) }))
            {
                var model = GetModelByGuid(guid);
                if (model == null)
                    throw new ApiException(HttpStatusCode.NotFound);

                var serviceModel = Map(model);

                AfterGet(serviceModel);

                return serviceModel;
            }
        }

        protected virtual void AfterGet(TServiceModel serviceModel)
        {
        }

        public virtual IList<Expression<Func<TServiceModel, object>>> GetUniqueFieldExpressions()
        {
            return null;
        }

        private Guid? GetDuplicateId(TServiceModel serviceModel)
        {
            var uniqueFieldExpressions = GetUniqueFieldExpressions();
            if (uniqueFieldExpressions == null) return null;

            var filterDescriptors = new List<FilterDescriptor>
            {
                new FilterDescriptor(nameof(ServiceModelBase.Guid), FilterOperator.Neq, serviceModel.Guid.ToString())
            };

            foreach (var uniqueFieldExpression in uniqueFieldExpressions)
            {
                var uniqueFieldFunc = uniqueFieldExpression.Compile();

                var fieldName = uniqueFieldExpression.GetPropertyName();
                var fieldValue = uniqueFieldFunc(serviceModel);

                var value = fieldValue is ServiceModelBase serviceModelBase ? serviceModelBase.Guid.ToString() : fieldValue.ToString();

                filterDescriptors.Add(new FilterDescriptor(fieldName, FilterOperator.Eq, value));
            }

            var filterExpressions = FilterExpressionBuilder.GetFilterExpression(filterDescriptors.ToArray());
            var result = DbSet.Where(x => !x.IsObsolete).FirstOrDefault(filterExpressions);

            return result?.Guid;
        }

        protected virtual async Task Adding(TServiceModel serviceModel)
        {
        }

        public virtual Guid Add(TServiceModel serviceModel)
        {
            using (Logger.LogMethodEnter(new[] { new MethodParameter("ServiceModel", serviceModel) }))
            {
                if (Equals(serviceModel.Guid, Guid.Empty))
                    serviceModel.Guid = Guid.NewGuid();

                var duplicateId = GetDuplicateId(serviceModel);
                if (duplicateId.HasValue)
                {
                    if (SilentlyIgnoreDuplicates)
                    {
                        return duplicateId.Value;
                    }

                    throw new ApiException(HttpStatusCode.Conflict, "Could not process request as this would cause duplicates.");
                }

                // TODO: make all calls async starting from the controller
                Task.Run(async () =>
                    {
                        await Adding(serviceModel);
                    })
                    .GetAwaiter()
                    .GetResult();

                BeforeCreate(serviceModel);
                
                var model = Map(serviceModel);

                DbSet.Add(model);

                BeforeSaveCreated(serviceModel, model);

                DataContext.SaveChanges();

                AfterCreate(serviceModel, model);

                return model.Guid;
            }
        }

        protected virtual bool CanDelete(TModel model)
        {
            return true;
        }

        public virtual void Delete(string id)
        {
            using (Logger.LogMethodEnter(new[] { new MethodParameter("Id", id) }))
            {
                var model = GetModelById(id);

                if (model == null)
                    throw new ApiException(HttpStatusCode.NotFound, "The item does not exists on the server");

                if (!CanDelete(model))
                {
                    throw new ApiException(HttpStatusCode.MethodNotAllowed, "The item cannot be deleted because it is still referenced by other entities");
                }

                BeforeDelete(model);

                model.IsObsolete = true;
                model.SetUpdateFields(ServiceContext.EntityId);

                BeforeSaveDeleted(model);

                DataContext.SaveChanges();

                AfterDelete(model);
            }
        }

        public virtual void Update(TServiceModel serviceModel)
        {
            using (Logger.LogMethodEnter(new[] { new MethodParameter("ServiceModel", serviceModel) }))
            {
                var model = GetModelByGuid(serviceModel.Guid);

                if (model == null)
                    throw new ApiException(HttpStatusCode.NotFound, "The item does not exists on the server");

                var duplicateId = GetDuplicateId(serviceModel);
                if (duplicateId.HasValue)
                {
                    if (SilentlyIgnoreDuplicates)
                        return;

                    throw new ApiException(HttpStatusCode.Conflict, "Could not process request as this would cause duplicates.");
                }

                BeforeUpdate(model, serviceModel);

                model = Map(serviceModel);

                BeforeSaveUpdated(serviceModel, model);

                DataContext.SaveChanges();

                AfterUpdate(serviceModel, model);
            }
        }

        /// <summary>
        /// Entry point for performing logic that needs to happen at the beginning of the update process
        /// </summary>
        public virtual void BeforeUpdate(TModel model, TServiceModel serviceModel)
        {
        }

        /// <summary>
        /// Entry point for performing logic that needs to happen before the updated entity will be saved to the database
        /// </summary>
        public virtual void BeforeSaveUpdated(TServiceModel serviceModel, TModel model)
        {
        }

        /// <summary>
        /// Entry point for performing logic that needs to happen after the updated entity has been saved to the database
        /// </summary>
        public virtual void AfterUpdate(TServiceModel serviceModel, TModel model)
        {
        }

        /// <summary>
        /// Entry point for performing logic that needs to happen at the beginning of the creation process
        /// </summary>
        public virtual void BeforeCreate(TServiceModel serviceModel)
        {
        }

        /// <summary>
        /// Entry point for performing logic that needs to happen before the newly created entity will be saved to the database
        /// </summary>
        public virtual void BeforeSaveCreated(TServiceModel serviceModel, TModel model)
        {
        }

        /// <summary>
        /// Entry point for performing logic that needs to happen after the newly created entity has been saved to the database
        /// </summary>
        public virtual void AfterCreate(TServiceModel serviceModel, TModel model)
        {
        }

        /// <summary>
        /// Entry point for performing logic that needs to happen at the beginning of the deletion process
        /// </summary>
        public virtual void BeforeDelete(TModel model)
        {
        }

        /// <summary>
        /// Entry point for performing logic that needs to happen before the obsolete entity will be saved to the database
        /// </summary>
        public virtual void BeforeSaveDeleted(TModel model)
        {
        }

        /// <summary>
        /// Entry point for performing logic that needs to happen after the obsolete entity has been saved to the database
        /// </summary>
        public virtual void AfterDelete(TModel model)
        {
        }

        public void Patch(Guid guid, JsonPatchDocument<TServiceModel> patch)
        {
            ServiceContext.Includes.IncludeAll = true;

            var model = GetModelByGuid(guid);

            if (model == null)
                throw new ApiException(HttpStatusCode.NotFound);

            var serviceModel = Map(model);

            patch.ApplyTo(serviceModel);
            Map(serviceModel, model);

            DataContext.SaveChanges();
        }

        public virtual ICommandResult Execute(CommandBase command)
        {
            throw new ApiException(HttpStatusCode.NotFound);
        }

        protected TModel GetModelById(string id)
        {
            return !Guid.TryParse(id, out var guid) ? null : GetModelByGuid(guid);
        }

        public TModel GetModelByGuid(Guid guid)
        {
            var query = ApplyEntityFrameworkIncludes(DbSet, false);

            return query.FirstOrDefault(x => x.Guid == guid);
        }

        #endregion

        #region mapping

        protected TModel Map(TServiceModel serviceModel)
        {
            BeforeMap(serviceModel);
            var model = Mapper.Map<TModel>(serviceModel, SetMappingOptions);
            AfterMap(serviceModel, model);

            return model;
        }

        protected TModel Map(TServiceModel serviceModel, TModel model)
        {
            BeforeMap(serviceModel);
            Mapper.Map(serviceModel, model, SetMappingOptions);
            AfterMap(serviceModel, model);

            return model;
        }

        protected TServiceModel Map(TModel model)
        {
            BeforeMap(model);
            var serviceModel = Mapper.Map<TServiceModel>(model, SetMappingOptions);
            AfterMap(model, serviceModel);

            return serviceModel;
        }

        protected virtual IQueryable<TModel> ApplyEntityFrameworkIncludes(IQueryable<TModel> query, bool fromSearch)
        {
            return query;
        }

        private IQueryable<TModel> ApplyIfModifiedSince(IQueryable<TModel> query, SearchParameters searchParameters)
        {
            if (searchParameters.IfModifiedSince.HasValue)
            {
                return query.Where(x => x.ModifyDate > searchParameters.IfModifiedSince);
            }

            return query;
        }

        protected virtual void BeforeMap(TServiceModel fromServiceModel)
        {
        }

        protected virtual void BeforeMap(TModel fromModel)
        {
        }

        protected virtual void AfterMap(TServiceModel fromServiceModel, TModel toModel)
        {
        }

        protected virtual void AfterMap(TModel fromModel, TServiceModel toServiceModel)
        {
        }

        protected void SetMappingOptions(IMappingOperationOptions options)
        {
            options.Items[AutoMapperExtensions.DbContextKey] = DataContext;
            options.Items[AutoMapperExtensions.IncludesKey] = ServiceContext?.Includes;
            options.Items[AutoMapperExtensions.UserNameKey] = ServiceContext?.EntityId;
        }

        #endregion

        #region search

        protected virtual Expression<Func<TModel, bool>> GetSearchExpression(string searchExpression)
        {
            return null;
        }

        protected IQueryable<TModel> ApplySearch(IQueryable<TModel> query, SearchParameters searchParameters)
        {
            if (searchParameters?.Search == null)
                return query;

            var searchExpression = GetSearchExpression(searchParameters.Search);

            if (searchExpression != null)
            {
                query = query.Where(searchExpression);
            }

            return query;
        }

        private IQueryable<TModel> ApplyPaging(IQueryable<TModel> query, out int startIndex, SearchParameters searchParameters)
        {
            startIndex = searchParameters?.Skip ?? 0;

            if (startIndex > 0) query = query.Skip(startIndex);

            var batchSize = searchParameters?.Take;

            if (batchSize.HasValue) query = query.Take(batchSize.Value);

            return query;
        }

        #endregion

        #region background tasks

        public BackgroundTaskStatus ReadBackgroundTaskStatus(Guid id)
        {
            return BackgroundTaskService.GetStatus(id);
        }

        protected static DataContext GetDataContext(bool useLazyLoading, bool autoDetectChanges)
        {
            var optionsBuilder = new DbContextOptionsBuilder<DataContext>();

            optionsBuilder.UseSqlServer(Settings.Current.ConnectionString);

            if (useLazyLoading)
            {
                optionsBuilder = optionsBuilder.UseLazyLoadingProxies();
            }

            var dataContext = new DataContext(optionsBuilder.Options);

            dataContext.ChangeTracker.AutoDetectChangesEnabled = autoDetectChanges;

            return dataContext;
        }

        #endregion

        public async Task<TChildModel> AddChild<TChildModel, TChildServiceModel>(Guid id,
            TChildServiceModel childServiceModel, Func<TModel, IList<TChildModel>> childSelector,
            Expression<Func<TChildModel, TModel>> backReferenceProperty) where TChildModel : ModelBase where TChildServiceModel : ServiceModelBase
        {
            if (childServiceModel == null)
                throw new ApiException(HttpStatusCode.BadRequest);

            var parentModel = await DbSet.GetAsync(id) ?? throw new ApiException(HttpStatusCode.NotFound);

            var childModel = Mapper.Map<TChildModel>(childServiceModel, SetMappingOptions);

            // set reference to parent
            childModel.SetPropertyValue(backReferenceProperty, parentModel);

            // add child to parent
            childSelector(parentModel).Add(childModel);

            return childModel;
        }
    }
}
