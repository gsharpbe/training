using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using AutoMapper;
using Metanous.Model.Core.Extensions;
using Metanous.Model.Core.Model;
using Microsoft.EntityFrameworkCore;
using Metanous.Model.Core.Search;

namespace Metanous.WebApi.Core.Extensions
{
    public static class AutoMapperExtensions
    {
        public static readonly string DbContextKey = typeof(DbContext).Name;
        public static readonly string IncludesKey = "Includes";
        public static readonly string ExcludesKey = "Excludes";
        public static readonly string UserNameKey = "UserName";

        #region model -> service model

        public static IMappingExpression<TModel, TServiceModel> CreateServiceModelMap<TModel, TServiceModel>(this IMapperConfigurationExpression config)
            where TModel : ModelBase
            where TServiceModel : ServiceModelBase
        {
            config.AllowNullCollections = true;
            return config.CreateMap<TModel, TServiceModel>().ApplyDefaultModelMappingOptions();
        }

        public static IMappingExpression<TModel, TServiceModel> ApplyDefaultModelMappingOptions<TModel, TServiceModel>(this IMappingExpression<TModel, TServiceModel> mappingExpression)
            where TServiceModel : ServiceModelBase
            where TModel : ModelBase
        {
            var serviceModelProperties = typeof(TServiceModel).GetProperties();
            var modelProperties = typeof(TModel).GetProperties();

            foreach (var propertyInfo in modelProperties)
            {
                if (typeof(ModelBase).IsAssignableFrom(propertyInfo.PropertyType))
                {

                    var serviceModelPropertyInfo = serviceModelProperties.FirstOrDefault(x => Equals(x.Name, propertyInfo.Name));

                    if (serviceModelPropertyInfo != null && typeof(ServiceModelBase).IsAssignableFrom(serviceModelPropertyInfo.PropertyType))
                    {
                        mappingExpression.ForMember(
                            propertyInfo.Name,
                            options =>
                            {
                                options.ResolveUsing((model, serviceModel, serviceModelMember, resolutionContext) =>
                                {
                                    if (IsPropertyIncluded<TServiceModel>(resolutionContext, propertyInfo.Name))
                                    {
                                        var item = propertyInfo.GetValue(model);
                                        if (item is ModelBase modelBase)
                                        {
                                            if (modelBase.IsObsolete)
                                                return null;
                                        }
                                        return propertyInfo.GetValue(model);
                                    }
                                    return null;
                                });
                            });
                    }
                }

                if (typeof(IEnumerable<ModelBase>).IsAssignableFrom(propertyInfo.PropertyType))
                {
                    var serviceModelPropertyInfo = serviceModelProperties.FirstOrDefault(x => Equals(x.Name, propertyInfo.Name));

                    if (serviceModelPropertyInfo != null && typeof(IEnumerable<ServiceModelBase>).IsAssignableFrom(serviceModelPropertyInfo.PropertyType))
                    {
                        mappingExpression.ForMember(
                            propertyInfo.Name,
                            options =>
                            {
                                options.ResolveUsing((model, serviceModel, serviceModelMember, resolutionContext) =>
                                {
                                    if (IsPropertyIncluded<TServiceModel>(resolutionContext, propertyInfo.Name))
                                    {
                                        var items = propertyInfo.GetValue(model);
                                        if (items is IEnumerable<ModelBase>)
                                        {
                                            items = ((IEnumerable<ModelBase>)items).Where(x => !x.IsObsolete);
                                        }
                                        return items;
                                    }
                                    return null;
                                });
                            });
                    }
                }
            }

            return mappingExpression.PreserveReferences();
        }

        #endregion

        #region service model -> model

        public static IMappingExpression<TServiceModel, TModel> CreateModelMap<TServiceModel, TModel>(this IMapperConfigurationExpression config)
            where TServiceModel : ServiceModelBase
            where TModel : ModelBase, new()
        {
            return config.CreateMap<TServiceModel, TModel>().ApplyDefaultServiceModelMappingOptions();
        }

        public static IMappingExpression<TServiceModel, TModel> ApplyDefaultServiceModelMappingOptions<TServiceModel, TModel>(this IMappingExpression<TServiceModel, TModel> mappingExpression)
            where TServiceModel : ServiceModelBase
            where TModel : ModelBase, new()
        {
            // try to load the destination object first in stead of creating a new instance immediately
            mappingExpression = mappingExpression.ConstructUsing((source, context) =>
            {
                var dbContext = context.GetDbContext();
                if (dbContext == null)
                    return new TModel();

                var repository = dbContext.Set<TModel>();

                var model = repository.FirstOrDefault(x => x.Guid == source.Guid) ?? new TModel();

                model.SetUpdateFields(context.GetUserName());

                return model;
            });

            return mappingExpression;
        }

        public static IMappingExpression<TServiceModel, TModel> MapReference<TServiceModel, TServiceModelProperty, TModel, TModelProperty>(
            this IMappingExpression<TServiceModel, TModel> mappingExpression,
            Expression<Func<TServiceModel, TServiceModelProperty>> sourceMember,
            Expression<Func<TModel, TModelProperty>> destinationMember, bool ignoreNull = false)
            where TServiceModel : ServiceModelBase
            where TServiceModelProperty : ServiceModelBase
            where TModel : ModelBase
            where TModelProperty : ModelBase
        {
            // in stead of mapping the source property to the destination property, try to retrieve the already stored destination object from the database
            mappingExpression = mappingExpression.ForMember(
                destinationMember,
                options => options.ResolveUsing((serviceModel, model, modelProperty, resolutionContext) =>
                {
                    if (ignoreNull)
                    {
                        var func = sourceMember.Compile();
                        var sourceValue = func(serviceModel);

                        if (sourceValue == null)
                            return modelProperty;
                    }

                    var value = serviceModel.FetchDestination<TServiceModel, TModelProperty, TServiceModelProperty>(
                            resolutionContext, sourceMember);

                    if (value == null)
                    {
                        var dataContext = GetDbContext(resolutionContext);
                        var entry = dataContext.Entry(model);

                        if (entry.State != EntityState.Detached)
                        {
                            entry.Reference(destinationMember.GetPropertyName()).Load();
                            model.SetPropertyValue(destinationMember, null);
                        }
                    }

                    return value;
                }));

            return mappingExpression;
        }

        public static IMappingExpression<TServiceModel, TModel> MapReference<TServiceModel, TServiceModelProperty, TModel, TModelProperty>(
            this IMappingExpression<TServiceModel, TModel> mappingExpression,
            Expression<Func<TServiceModel, IList<TServiceModelProperty>>> sourceMember,
            Expression<Func<TModel, IList<TModelProperty>>> destinationMember)
            where TServiceModel : ServiceModelBase
            where TServiceModelProperty : ServiceModelBase
            where TModel : ModelBase
            where TModelProperty : ModelBase
        {
            // in stead of mapping the source property to the destination property, try to retrieve the already stored destination object from the database
            mappingExpression = mappingExpression.ForMember(
                destinationMember,
                options => options.ResolveUsing((serviceModel, model, modelProperty, resolutionContext) => serviceModel.FetchDestination(model, resolutionContext, sourceMember, destinationMember)));

            return mappingExpression;
        }

        public static IMappingExpression<TServiceModel, TModel> MapBackReference<TServiceModel, TModel, TChild>(
            this IMappingExpression<TServiceModel, TModel> mappingExpression,
            Func<TModel, IList<TChild>> childSelector,
            Expression<Func<TChild, TModel>> backReferenceProperty)
        {
            // get a function for retrieving the backreference value of an object
            var getValue = backReferenceProperty.Compile();

            // fill all missing back references of the items in the collection
            mappingExpression = mappingExpression.AfterMap((src, dest) =>
            {
                foreach (var child in childSelector(dest))
                {
                    if (getValue(child) == null)
                    {
                        child.SetPropertyValue(backReferenceProperty, dest);
                    }
                }
            });

            return mappingExpression;
        }

        #endregion

        #region general

        public static IMappingExpression<TSource, TDestination> Ignore<TSource, TDestination>(this IMappingExpression<TSource, TDestination> mappingExpression, Expression<Func<TDestination, object>> destination)
        {
            mappingExpression = mappingExpression.ForMember(destination, options => options.Ignore());
            return mappingExpression;
        }

        #endregion

        #region helpers

        private static TModelProperty FetchDestination<TServiceModel, TModelProperty, TServiceModelProperty>(
            this TServiceModel serviceModel,
            ResolutionContext resolutionContext,
            Expression<Func<TServiceModel, TServiceModelProperty>> selector)
            where TServiceModel : ServiceModelBase
            where TModelProperty : ModelBase
            where TServiceModelProperty : ServiceModelBase
        {
            var dbContext = GetDbContext(resolutionContext);

            if (dbContext == null)
                return null;

            // get guid of source value
            var func = selector.Compile();
            var sourceValue = func(serviceModel);

            if (sourceValue == null)
                return null;

            // look it up in the database
            var repository = dbContext.Set<TModelProperty>();
            var model = repository.FirstOrDefault(x => x.Guid == sourceValue.Guid);

            return model;
        }

        private static IList<TModelProperty> FetchDestination<TServiceModel, TModel, TModelProperty, TServiceModelProperty>(
            this TServiceModel serviceModel,
            TModel model,
            ResolutionContext resolutionContext,
            Expression<Func<TServiceModel, IList<TServiceModelProperty>>> sourceMember,
            Expression<Func<TModel, IList<TModelProperty>>> destinationMember)
            where TServiceModel : ServiceModelBase
            where TModel : ModelBase
            where TModelProperty : ModelBase
            where TServiceModelProperty : ServiceModelBase
        {
            var dbContext = GetDbContext(resolutionContext);

            if (dbContext == null)
                return null;

            // get guid of source value
            var sourceValue = sourceMember.Compile()(serviceModel);

            if (sourceValue == null)
                return null;

            // get guid of source value
            var destinationValue = destinationMember.Compile()(model);

            if (destinationValue == null)
                return null;

            // look it up in the database
            var repository = dbContext.Set<TModelProperty>();

            foreach (var destinationItem in destinationValue.ToArray())
            {
                if (!sourceValue.Any(x => Equals(x.Guid, destinationItem.Guid)))
                {
                    destinationValue.Remove(destinationItem);
                }
            }

            foreach (var sourceItem in sourceValue)
            {
                if (!destinationValue.Any(x => Equals(x.Guid, sourceItem.Guid)))
                {
                    var destinationItem = repository.FirstOrDefault(x => x.Guid == sourceItem.Guid);

                    if (destinationItem != null)
                        destinationValue.Add(destinationItem);
                }
            }

            return destinationValue;
        }

        public static string GetUserName(this ResolutionContext context)
        {
            if (context.Options.Items.TryGetValue(UserNameKey, out object userName))
                return userName as string;

            return null;
        }

        public static DbContext GetDbContext(this ResolutionContext context)
        {
            if (context.Options.Items.TryGetValue(DbContextKey, out object dbContext))
                return dbContext as DbContext;

            return null;
        }

        public static bool IsPropertyIncluded<TServiceModel>(this ResolutionContext resolutionContext, string propertyName) where TServiceModel : ServiceModelBase
        {
            var excludes = GetExcludes(resolutionContext);
            if (excludes != null && excludes.Has<TServiceModel>(propertyName))
            {
                return false;
            }

            var includes = GetIncludes(resolutionContext);

            return includes == null || includes.Has<TServiceModel>(propertyName);
        }

        public static bool IsApplyExplicitLoading(this ResolutionContext resolutionContext)
        {
            var includes = GetIncludes(resolutionContext);

            return includes?.ApplyExplicitLoading ?? false;
        }

        public static bool IsPropertyIncluded<TServiceModel>(this ResolutionContext resolutionContext, Expression<Func<TServiceModel, object>> property) where TServiceModel : ServiceModelBase
        {
            var includes = GetIncludes(resolutionContext);

            return includes == null || includes.Has(property);
        }

        public static Includes GetIncludes(this ResolutionContext context)
        {
            if (context.Options.Items.TryGetValue(IncludesKey, out var includes))
                return includes as Includes;

            return new Includes();
        }

        public static Includes GetExcludes(this ResolutionContext context)
        {
            if (context.Options.Items.TryGetValue(ExcludesKey, out var excludes))
                return excludes as Includes;

            return new Includes();
        }

        public static T Map<T>(this ResolutionContext context, object obj, Includes includes = null)
        {
            return context.Mapper.Map<T>(obj, x =>
            {
                foreach (var key in context.Options.Items.Keys)
                {
                    if (key == IncludesKey && includes != null)
                    {
                        x.Items[key] = includes;
                    }
                    else
                    {
                        x.Items[key] = context.Options.Items[key];
                    }
                }
            });
        }

        #endregion
    }
}