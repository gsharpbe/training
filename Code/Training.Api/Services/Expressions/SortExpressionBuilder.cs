using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Metanous.Model.Core.Model;
using Metanous.Model.Core.Search;
using Metanous.Model.Core.Sort;
using Training.Api.Controllers.Base;

namespace Training.Api.Services.Expressions
{
    public class SortExpressionBuilder<TModel> where TModel: ModelBase
    {
        private readonly Lazy<PropertyInfo[]> _propertyInfos;

        public ServiceContext ServiceContext;

        public SortExpressionBuilder()
        {
            _propertyInfos = new Lazy<PropertyInfo[]>(() => typeof(TModel).GetProperties());
        }

        #region api

        public IQueryable<TModel> ApplySorting(IQueryable<TModel> query, SearchParameters searchParameters)
        {
            var isFirst = true;

            if (searchParameters?.Sort?.Any() != true)
            {
                var defaultSortProperty = GetDefaultSortProperty();
                if (defaultSortProperty != null)
                {
                    query = GetDefaultSortDirection() == SortDirection.Asc
                        ? query.OrderBy(defaultSortProperty)
                        : query.OrderByDescending(defaultSortProperty);
                }
                else
                {
                    query = query.OrderBy(x => x.Id);
                }
            }
            else
            {
                if (searchParameters.Sort == null)
                    return null;

                foreach (var sortDescriptor in searchParameters.Sort)
                {
                    var property = GetCustomSortExpression(sortDescriptor.Field) ?? GetSortExpression(sortDescriptor.Field);

                    if (property != null)
                    {
                        if (isFirst)
                        {
                            query = sortDescriptor.Direction == SortDirection.Asc
                                ? query.OrderBy(property)
                                : query.OrderByDescending(property);

                            isFirst = false;
                        }
                        else
                        {
                            query = sortDescriptor.Direction == SortDirection.Asc
                                ? ((IOrderedQueryable<TModel>)query).ThenBy(property)
                                : ((IOrderedQueryable<TModel>)query).ThenByDescending(property);
                        }
                    }
                }
            }

            return query;
        }

        #endregion

        #region virtual

        protected virtual Expression<Func<TModel, object>> GetDefaultSortProperty()
        {
            return null;
        }

        protected virtual SortDirection GetDefaultSortDirection()
        {
            return SortDirection.Asc;
        }

        protected virtual Expression<Func<TModel, object>> GetCustomSortExpression(string field)
        {
            return null;
        }

        #endregion

        #region helpers
        
        private Expression<Func<TModel, object>> GetSortExpression(string field)
        {
            var propertyInfo = _propertyInfos.Value.FirstOrDefault(x => x.Name.Equals(field, StringComparison.InvariantCultureIgnoreCase));

            if (propertyInfo == null)
                return null;

            var parameterExpression = Expression.Parameter(typeof(TModel), "x");
            var propertyExpression = Expression.Property(parameterExpression, propertyInfo);
            var convertExpression = Expression.Convert(propertyExpression, typeof(object));

            return Expression.Lambda<Func<TModel, object>>(convertExpression, parameterExpression);
        }

        #endregion
    }
}
