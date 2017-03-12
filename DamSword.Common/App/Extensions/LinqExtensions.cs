using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace DamSword.Common
{
    public enum LinqWhereOperand
    {
        Equal,
        NotEqual,
        GreaterThan,
        GreaterThanOrEqual,
        LessThan,
        LessThanOrEqual,
        Contains
    }

    public static class LinqExtensions
    {
        public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> source, string property)
        {
            return ApplyOrder(source, property, "OrderBy");
        }
        public static IOrderedQueryable<T> OrderByDescending<T>(this IQueryable<T> source, string property)
        {
            return ApplyOrder(source, property, "OrderByDescending");
        }
        public static IOrderedQueryable<T> ThenBy<T>(this IOrderedQueryable<T> source, string property)
        {
            return ApplyOrder(source, property, "ThenBy");
        }
        public static IOrderedQueryable<T> ThenByDescending<T>(this IOrderedQueryable<T> source, string property)
        {
            return ApplyOrder(source, property, "ThenByDescending");
        }
        private static IOrderedQueryable<T> ApplyOrder<T>(IQueryable<T> source, string property, string methodName)
        {
            var props = property.Split('.');
            var type = typeof(T);
            var arg = Expression.Parameter(type, "x");
            Expression parameterExpression = arg;
            foreach (var prop in props)
            {
                // use reflection (not ComponentModel) to mirror LINQ
                var propertyInfo = type.GetProperty(prop);
                parameterExpression = Expression.Property(parameterExpression, propertyInfo);
                type = propertyInfo.PropertyType;
            }
            var delegateType = typeof(Func<,>).MakeGenericType(typeof(T), type);
            var lambda = Expression.Lambda(delegateType, parameterExpression, arg);

            var result = typeof(Queryable).GetMethods().Single(
                    method => method.Name == methodName
                            && method.IsGenericMethodDefinition
                            && method.GetGenericArguments().Length == 2
                            && method.GetParameters().Length == 2)
                    .MakeGenericMethod(typeof(T), type)
                    .Invoke(null, new object[] { source, lambda });

            return (IOrderedQueryable<T>)result;
        }


        public static IQueryable<T> Where<T, TProperty>(this IQueryable<T> self, string propertyName, LinqWhereOperand operand, TProperty comparator)
        {
            var type = typeof(T);
            var property = type.GetProperty(propertyName);
            var parameterExpr = Expression.Parameter(type, "p");
            var propertyAccessExpr = Expression.MakeMemberAccess(parameterExpr, property);
            var convertedComparator = typeof(TProperty) == property.PropertyType ? comparator : comparator.ToType(property.PropertyType);

            Expression compareExpr;
            switch (operand)
            {
                case LinqWhereOperand.Equal:
                    compareExpr = Expression.Equal(propertyAccessExpr, Expression.Constant(convertedComparator));
                    break;
                case LinqWhereOperand.NotEqual:
                    compareExpr = Expression.NotEqual(propertyAccessExpr, Expression.Constant(convertedComparator));
                    break;
                case LinqWhereOperand.GreaterThan:
                    compareExpr = Expression.GreaterThan(propertyAccessExpr, Expression.Constant(convertedComparator));
                    break;
                case LinqWhereOperand.GreaterThanOrEqual:
                    compareExpr = Expression.GreaterThanOrEqual(propertyAccessExpr, Expression.Constant(convertedComparator));
                    break;
                case LinqWhereOperand.LessThan:
                    compareExpr = Expression.LessThan(propertyAccessExpr, Expression.Constant(convertedComparator));
                    break;
                case LinqWhereOperand.LessThanOrEqual:
                    compareExpr = Expression.LessThanOrEqual(propertyAccessExpr, Expression.Constant(convertedComparator));
                    break;
                case LinqWhereOperand.Contains:
                    var containsMetodInfo = typeof(string).GetMethod("Contains");
                    var stringPropertyAccessExpr = property.PropertyType == typeof(string)
                        ? (Expression)propertyAccessExpr
                        : Expression.Convert(propertyAccessExpr, typeof(string));

                    var stringComparator = typeof(TProperty) == typeof(string)
                        ? convertedComparator as string
                        : convertedComparator?.ToString();

                    compareExpr = Expression.Call(stringPropertyAccessExpr, containsMetodInfo, Expression.Constant(stringComparator));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(operand), operand, null);
            }

            var conditionExpr = Expression.Lambda(compareExpr, parameterExpr);
            var whereMethodInfo = typeof(Queryable).GetMethods()
                .First(m => m.Name == "Where") // TODO: implement safe Single "Where" method signature predicate
                .MakeGenericMethod(typeof(T));

            var resultExpr = Expression.Call(null, whereMethodInfo, self.Expression, Expression.Quote(conditionExpr));
            return self.Provider.CreateQuery<T>(resultExpr);
        }
    }
}
