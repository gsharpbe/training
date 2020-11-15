using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Metanous.Model.Core.Extensions
{
    public static class ExpressionExtensions
    {
        public static string GetPropertyName<T, TP>(this Expression<Func<T, TP>> expression)
        {
            var unaryExpression = expression.Body as UnaryExpression;
            if (unaryExpression != null)
            {
                var memberExpression = unaryExpression.Operand as MemberExpression;
                return memberExpression?.Member.Name;
            }
            
            return (expression.Body as MemberExpression)?.Member.Name;

        }

        public static void SetPropertyValue<T, TK>(this T target, Expression<Func<T, TK>> memberLamda, TK value)
        {
            var memberSelectorExpression = memberLamda.Body as MemberExpression;
            if (memberSelectorExpression == null) return;

            var property = memberSelectorExpression.Member as PropertyInfo;
            property?.SetValue(target, value, null);
        }

        public static object GetPropertyValue<T>(this T target, string propertyName)
        {
            return target.GetType().GetProperty(propertyName).GetValue(target);
        }
    }
}