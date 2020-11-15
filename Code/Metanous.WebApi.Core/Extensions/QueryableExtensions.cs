using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Metanous.WebApi.Core.Extensions
{
    public static class QueryableExtensions
    {
        public static IQueryable<TResult> Select<TResult>(this IQueryable source, string[] columns)
        {
            var sourceType = source.ElementType;
            var resultType = typeof(TResult);
            var parameter = Expression.Parameter(sourceType, "e");
            var bindings = columns.Select(column => Expression.Bind(
                resultType.GetProperty(column), Expression.PropertyOrField(parameter, column)));
            var body = Expression.MemberInit(Expression.New(resultType), bindings);
            var selector = Expression.Lambda(body, parameter);

            return source.Provider.CreateQuery<TResult>(
                Expression.Call(typeof(Queryable), "Select", new Type[] { sourceType, resultType },
                    source.Expression, Expression.Quote(selector)));
        }

        public static IQueryable<T> Select<T>(this IQueryable<T> source, List<Expression<Func<T, object>>> columns)
        {
            var members = columns.Select(column => ((MemberExpression)column.Body).Member).ToList();

            var sourceType = typeof(T);
            var resultType = typeof(T);
            var parameter = Expression.Parameter(sourceType, "e");
            var bindings = members.Select(member => Expression.Bind(
                resultType.GetProperty(member.Name), Expression.MakeMemberAccess(parameter, member)));
            var body = Expression.MemberInit(Expression.New(resultType), bindings);
            var selector = Expression.Lambda<Func<T, T>>(body, parameter);
            return source.Select(selector);
        }

        public static IQueryable<IGrouping<object, T>> GroupBy2<T>(this IQueryable<T> query, string[] propertyNames)
        {
            var properties = propertyNames.Select(name => typeof(T).GetProperty(name)).ToArray();
            var propertyTypes = properties.Select(p => p.PropertyType).ToArray();
            var tupleTypeDefinition = typeof(Tuple).Assembly.GetType("System.Tuple`" + properties.Length);
            var tupleType = tupleTypeDefinition.MakeGenericType(propertyTypes);
            var constructor = tupleType.GetConstructor(propertyTypes);
            var param = Expression.Parameter(typeof(T), "item");
            var body = Expression.New(constructor, properties.Select(p => Expression.Property(param, p)));
            var expr = Expression.Lambda<Func<T, object>>(body, param);

            return query.GroupBy(expr);
        }
    }
}