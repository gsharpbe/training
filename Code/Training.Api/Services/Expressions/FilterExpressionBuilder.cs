using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Metanous.Model.Core.Extensions;
using Metanous.Model.Core.Filter;
using Metanous.Model.Core.Model;
using Metanous.Model.Core.Search;
using Training.Api.Controllers.Base;

namespace Training.Api.Services.Expressions
{
    public class FilterExpressionBuilder<TModel> where TModel: ModelBase
    {
        internal class MethodDefinitions
        {
            internal readonly MethodInfo IsNullOrEmptyMethodInfo = typeof(string).GetMethod("IsNullOrEmpty", new[] { typeof(string) });
            internal readonly MethodInfo StartsWithMethodInfo = typeof(string).GetMethod("StartsWith", new[] { typeof(string) });
            internal readonly MethodInfo EndsWithMethodInfo = typeof(string).GetMethod("EndsWith", new[] { typeof(string) });
            internal readonly MethodInfo ContainsMethodInfo = typeof(string).GetMethod("Contains", new[] { typeof(string) });
            internal readonly MethodInfo ToUpperMethodInfo = typeof(string).GetMethod("ToUpper", new Type[] { });
        }

        private readonly Lazy<PropertyInfo[]> _propertyInfos;
        private readonly MethodDefinitions _methodDefinitions;

        public ServiceContext ServiceContext;

        public FilterExpressionBuilder()
        {
            _propertyInfos = new Lazy<PropertyInfo[]>(() => typeof(TModel).GetProperties());
            _methodDefinitions = new MethodDefinitions();
        }

        #region api

        public IQueryable<TModel> ApplyFiltering(IQueryable<TModel> query, SearchParameters searchParameters)
        {
            var globalFilterExpression = GetGlobalFilterExpression();
            if (globalFilterExpression != null)
            {
                query = query.Where(globalFilterExpression);
            }

            var childFilterExpressions = GetChildFilterExpressions(searchParameters?.Filter);
            if (childFilterExpressions != null)
            {
                foreach (var childFilterExpression in childFilterExpressions)
                {
                    query = query.Where(childFilterExpression);
                }
            }

            if (searchParameters?.Filter != null)
            {
                var filterExpression = GetFilterExpression(searchParameters.Filter.ToArray());

                if (filterExpression != null) query = query.Where(filterExpression);
            }

            return query;
        }

        protected virtual IEnumerable<Expression<Func<TModel, bool>>> GetChildFilterExpressions(FilterDescriptor[] filterDescriptors)
        {
            return null;
        }

        public Expression<Func<TModel, bool>> GetFilterExpression(FilterDescriptor[] filterDescriptors)
        {
            var parameterExpression = Expression.Parameter(typeof(TModel), "x");

            var filterExpression = GetFilterExpression(filterDescriptors, parameterExpression);

            if (filterExpression == null) return null;

            return Expression.Lambda<Func<TModel, bool>>(filterExpression, parameterExpression);
        }

        public Expression<Func<TModel, bool>> GetCombinedFilterExpression(FilterDescriptor[] filterDescriptors)
        {
            var childFilterExpressions = GetChildFilterExpressions(filterDescriptors) ?? new List<Expression<Func<TModel, bool>>>();
            var filterExpressions = childFilterExpressions.Where(x => x != null).ToList();
            var filterExpression = GetFilterExpression(filterDescriptors);

            if (filterExpression != null)
            {
                filterExpressions.Add(filterExpression);
            }

            Expression<Func<TModel, bool>> combinedFilterExpression = null;
            foreach (var expression in filterExpressions)
            {
                if (combinedFilterExpression == null)
                {
                    combinedFilterExpression = expression;
                }
                else
                {
                    combinedFilterExpression = CombineAnd(combinedFilterExpression, expression);
                }
            }

            return combinedFilterExpression;
        }

        #endregion

        #region virtual

        /// <summary>
        /// Specific override in case of field names that don't match any model property names / different behaviour
        /// </summary>
        /// <param name="field"></param>
        /// <param name="value"></param>
        /// <param name="filterOperator"></param>
        /// <returns></returns>
        protected virtual Expression<Func<TModel, bool>> GetCustomFilterExpression(string field, string value, FilterOperator filterOperator)
        {
            return null;
        }

        /// <summary>
        /// Override if the filter field name shoudl map on a different member
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        protected virtual Expression<Func<TModel, object>> GetCustomFilterFieldMappingExpression(string field)
        {
            return null;
        }

        /// <summary>
        /// Injects a 'where' statement in every search
        /// </summary>
        /// <returns></returns>
        protected virtual Expression<Func<TModel, bool>> GetGlobalFilterExpression()
        {
            return null;
        }

        #endregion

        #region helpers

        private Expression GetFilterExpression(FilterDescriptor[] filterDescriptors, ParameterExpression parameterExpression)
        {
            if (filterDescriptors == null || !filterDescriptors.Any())
                return null;

            Expression expression = null;

            foreach (var filterDescriptor in filterDescriptors.Where(x=>x.Field != null))
            {
                var otherExpression = GetFilterExpression(filterDescriptor.Field, filterDescriptor.Value,
                    filterDescriptor.Operator, parameterExpression);

                if (otherExpression == null)
                    continue;

                expression = expression != null ? Expression.AndAlso(expression, otherExpression) : otherExpression;
            }

            return expression;
        }

        private Expression GetFilterExpression(string field, string value, FilterOperator filterOperator, ParameterExpression parameterExpression)
        {
            var customFilterExpression = GetCustomFilterExpression(field, value, filterOperator);
            if (customFilterExpression != null)
                return new ParameterVisitor(customFilterExpression.Parameters, new ReadOnlyCollection<ParameterExpression>(new List<ParameterExpression>
                {
                    parameterExpression
                })).VisitAndConvert(customFilterExpression.Body, "");

            Expression member;
            Type memberType;

            var customFilterFieldMappingExpression = GetCustomFilterFieldMappingExpression(field);
            if (customFilterFieldMappingExpression != null)
            {
                var body = customFilterFieldMappingExpression.Body;

                if (body.NodeType == ExpressionType.Convert) body = ((UnaryExpression)body).Operand;

                memberType = body.Type;

                member = new ParameterVisitor(customFilterFieldMappingExpression.Parameters, new ReadOnlyCollection<ParameterExpression>(new List<ParameterExpression>
                {
                    parameterExpression
                })).VisitAndConvert(body, "");
            }
            else
            {
                var propertyInfo = _propertyInfos.Value.FirstOrDefault(x => x.Name.Equals(field, StringComparison.InvariantCultureIgnoreCase));

                if (propertyInfo == null) return null;


                memberType = propertyInfo.PropertyType;

                member = Expression.Property(parameterExpression, propertyInfo);
            }

            if (typeof(ModelBase).IsAssignableFrom(memberType) && filterOperator != FilterOperator.IsNull && filterOperator != FilterOperator.IsNotNull)
            {
                memberType = typeof(Guid);
                member = ConvertExpressionToGuid(member, parameterExpression).Body;
            }



            if (member == null) return null;

            switch (filterOperator)
            {
                case FilterOperator.Eq:
                    {
                        ConstantExpression constantExpression;
                        if (memberType == typeof(string))
                        {
                            constantExpression = Expression.Constant(value, memberType);
                        }
                        else
                        {
                            constantExpression = GetConvertConstantExpression(memberType, value);
                            if (constantExpression == null) return null;
                        }
                        return Expression.Equal(member, constantExpression);
                    }

                case FilterOperator.Neq:
                    {
                        ConstantExpression constantExpression;
                        if (memberType == typeof(string))
                        {
                            constantExpression = Expression.Constant(value, memberType);
                        }
                        else
                        {
                            constantExpression = GetConvertConstantExpression(memberType, value);
                            if (constantExpression == null) return null;
                        }
                        return Expression.NotEqual(member, constantExpression);
                    }

                case FilterOperator.IsNull:
                    {
                        bool canBeNull = !memberType.IsValueType || (Nullable.GetUnderlyingType(memberType) != null);
                        if (!canBeNull) return null;
                        return Expression.Equal(member, Expression.Constant(null, memberType));
                    }

                case FilterOperator.IsNotNull:
                    {
                        bool canBeNull = !memberType.IsValueType || (Nullable.GetUnderlyingType(memberType) != null);
                        if (!canBeNull) return null;
                        return Expression.NotEqual(member, Expression.Constant(null, memberType));
                    }

                case FilterOperator.Gt:
                    {
                        var constantExpression = GetConvertConstantExpression(memberType, value);
                        if (constantExpression == null) return null;
                        return Expression.GreaterThan(member, constantExpression);
                    }

                case FilterOperator.Gte:
                    {
                        var constantExpression = GetConvertConstantExpression(memberType, value);
                        if (constantExpression == null) return null;
                        return Expression.GreaterThanOrEqual(member, constantExpression);
                    }

                case FilterOperator.Lt:
                    {
                        var constantExpression = GetConvertConstantExpression(memberType, value);
                        if (constantExpression == null) return null;
                        return Expression.LessThan(member, constantExpression);
                    }

                case FilterOperator.Lte:
                    {
                        var constantExpression = GetConvertConstantExpression(memberType, value);
                        if (constantExpression == null) return null;
                        return Expression.LessThanOrEqual(member, constantExpression);
                    }

                case FilterOperator.Contains:
                    {
                        if (typeof(string) == memberType)
                        {
                            var constantExpression = Expression.Constant(value, memberType);
                            return Expression.Call(member, _methodDefinitions.ContainsMethodInfo, constantExpression);
                        }
                        if (typeof(Guid) == memberType)
                        {
                            var expressions = value.ToGuids().Select(item =>
                            {
                                var constantExpression = Expression.Constant(item, typeof(Guid));
                                var equalExpression = Expression.Equal(member, constantExpression);
                                return (Expression)equalExpression;
                            });
                            return expressions.Aggregate(Expression.Or);
                        }
                        if (memberType.IsEnum)
                        {
                            var expressions = value.ToEnums(memberType).Select(item =>
                            {
                                var constantExpression = Expression.Constant(item, memberType);
                                var equalExpression = Expression.Equal(member, constantExpression);
                                return (Expression)equalExpression;
                            });
                            return expressions.Aggregate(Expression.Or);
                        }

                        return null;
                    }

                case FilterOperator.DoesNotContain:
                    {
                        if (typeof(string) == memberType)
                        {
                            var constantExpression = Expression.Constant(value, memberType);
                            return Expression.Not(Expression.Call(member, _methodDefinitions.ContainsMethodInfo, constantExpression));
                        }
                        if (typeof(Guid) == memberType)
                        {
                            var expressions = value.ToGuids().Select(item =>
                            {
                                var constantExpression = Expression.Constant(item, typeof(Guid));
                                var equalExpression = Expression.NotEqual(member, constantExpression);
                                return (Expression)equalExpression;
                            });
                            return expressions.Aggregate(Expression.And);
                        }

                        return null;

                    }

                case FilterOperator.StartsWith:
                    {
                        if (typeof(string) != memberType) return null;
                        var constantExpression = Expression.Constant(value, memberType);
                        return Expression.Call(member, _methodDefinitions.StartsWithMethodInfo, constantExpression);
                    }

                case FilterOperator.EndsWith:
                    {
                        if (typeof(string) != memberType) return null;
                        var constantExpression = Expression.Constant(value, memberType);
                        return Expression.Call(member, _methodDefinitions.EndsWithMethodInfo, constantExpression);
                    }

                case FilterOperator.IsEmpty:
                    return Expression.Call(_methodDefinitions.IsNullOrEmptyMethodInfo, member);

                case FilterOperator.IsNotEmpty:
                    return Expression.Not(Expression.Call(_methodDefinitions.IsNullOrEmptyMethodInfo, member));

                default:
                    return null;
            }
        }

        private static Expression<Func<TModel, Guid>> ConvertExpressionToGuid(Expression member, ParameterExpression parameterExpression)
        {
            var body = Expression.Property(member, "Guid");
            var expression = Expression.Lambda<Func<TModel, Guid>>(body, Expression.Lambda(member, parameterExpression).Parameters[0]);

            return expression;
        }

        private static ConstantExpression GetConvertConstantExpression(Type propertyType, string value)
        {
            var underlyingType = Nullable.GetUnderlyingType(propertyType) ?? propertyType;

            if (underlyingType == typeof(Guid))
            {
                if (Guid.TryParse(value, out var converted))
                {
                    return Expression.Constant(converted, propertyType);
                }
            }

            if (underlyingType == typeof(DateTimeOffset))
            {
                if (DateTimeOffset.TryParse(value, out var converted))
                {
                    return Expression.Constant(converted, propertyType);
                }
            }

            if (underlyingType.IsEnum)
            {
                if (Enum.TryParse(underlyingType, value, true, out var converted))
                {
                    return Expression.Constant(converted, propertyType);
                }
            }

            try
            {
                var converted = value == null
                    ? null
                    : Convert.ChangeType(value, underlyingType, CultureInfo.InvariantCulture);

                return Expression.Constant(converted, propertyType);
            }
            catch (Exception)
            {
                // ignored
            }

            try
            {
                var converted = TypeDescriptor.GetConverter(underlyingType).ConvertFromInvariantString(value);

                return Expression.Constant(converted, propertyType);
            }
            catch (Exception)
            {
                // ignored
            }

            return null;
        }

        private class ParameterVisitor : ExpressionVisitor
        {
            private readonly ReadOnlyCollection<ParameterExpression> _from, _to;
            public ParameterVisitor(
                ReadOnlyCollection<ParameterExpression> from,
                ReadOnlyCollection<ParameterExpression> to)
            {
                if (from == null) throw new ArgumentNullException(nameof(@from));
                if (to == null) throw new ArgumentNullException(nameof(to));
                if (from.Count != to.Count) throw new InvalidOperationException("Parameter lengths must match");
                _from = from;
                _to = to;
            }
            protected override Expression VisitParameter(ParameterExpression node)
            {
                for (var i = 0; i < _from.Count; i++)
                {
                    if (node == _from[i]) return _to[i];
                }
                return node;
            }
        }

        private class ExpressionParameterReplacer : ExpressionVisitor
        {
            public ExpressionParameterReplacer(IList<ParameterExpression> fromParameters, IList<ParameterExpression> toParameters)
            {
                ParameterReplacements = new Dictionary<ParameterExpression, ParameterExpression>();
                for (var i = 0; i != fromParameters.Count && i != toParameters.Count; i++)
                    ParameterReplacements.Add(fromParameters[i], toParameters[i]);
            }

            private IDictionary<ParameterExpression, ParameterExpression> ParameterReplacements
            {
                get;
            }
            protected override Expression VisitParameter(ParameterExpression node)
            {
                if (ParameterReplacements.TryGetValue(node, out var replacement))
                    node = replacement;
                return base.VisitParameter(node);
            }
        }

        protected IEnumerable<FilterDescriptor> GetFilterDescriptors(IEnumerable<FilterDescriptor> filterDescriptors,
            IEnumerable<string> fields)
        {
            return filterDescriptors.Where(x => x.Field != null && fields.Select(y => y.ToLower()).Contains(x.Field.ToLower()));
        }

        protected Expression<Func<TModel, bool>> CombineOr(Expression<Func<TModel, bool>> expr1, Expression<Func<TModel, bool>> expr2)
        {
            return Combine(Expression.Or, expr1, expr2);
        }

        protected Expression<Func<TModel, bool>> CombineAnd(Expression<Func<TModel, bool>> expr1, Expression<Func<TModel, bool>> expr2)
        {
            return Combine(Expression.And, expr1, expr2);
        }

        private static Expression<Func<TModel, bool>> Combine(Func<Expression, Expression, BinaryExpression> combineFunc, Expression<Func<TModel, bool>> expr1, Expression<Func<TModel, bool>> expr2)
        {
            var expression = combineFunc(expr1.Body,
                new ExpressionParameterReplacer(expr2.Parameters, expr1.Parameters).Visit(expr2.Body));

            return Expression.Lambda<Func<TModel, bool>>(expression, expr1.Parameters);
        }



        #endregion
    }
}
