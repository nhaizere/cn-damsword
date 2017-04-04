using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace DamSword.Common
{
    public static class ExpressionExtensions
    {
        public static Expression SkipConvert(this Expression expression)
        {
            return expression.NodeType == ExpressionType.Convert ? (expression as UnaryExpression)?.Operand : expression;
        }

        public static Expression<T> Not<T>(this Expression<T> self)
        {
            if (self.NodeType != ExpressionType.Lambda)
                throw new ArgumentException("Must contain lambda expression.", nameof(self));

            var invertedConditionExpression = Expression.Not(self.Body.SkipConvert());
            if (self.ReturnType != invertedConditionExpression.Type)
                invertedConditionExpression = Expression.Convert(invertedConditionExpression, self.ReturnType);

            return Expression.Lambda<T>(invertedConditionExpression, self.Parameters);
        }

        public static bool HasParameterExpression(this Expression self)
        {
            var constantVisitor = new ParameterVisitor();
            constantVisitor.Visit(self);

            return constantVisitor.HasParameterExpression;
        }

        public static object EvaluateConstExpression(this Expression self)
        {
            var convertExpr = Expression.Convert(self, typeof(object));
            var lambdaExpr = Expression.Lambda(convertExpr);
            var value = lambdaExpr.Compile().DynamicInvoke();
            return value;
        }

        public static TExpression EvaluateStaticValuesToConstants<TExpression>(this TExpression self)
            where TExpression : Expression
        {
            var checkVisitor = new EvaluateStaticValuesToConstantsVisitor();
            return (TExpression)checkVisitor.Visit(self);
        }

        public static TProperty EvaluateProperty<TModel, TProperty>(this Expression<Func<TModel, TProperty>> self, TModel value)
        {
            return self.Compile().Invoke(value);
        }

        public static object EvaluateProperty<TModel>(this Expression<Func<TModel, object>> self, TModel value)
        {
            return self.Compile().Invoke(value);
        }

        public static MemberExpression AsMemberExpressionSkipConvert(this Expression expression)
        {
            var expressionBody = (expression as LambdaExpression)?.Body ?? expression;
            return expressionBody as MemberExpression ?? (expressionBody as UnaryExpression)?.Operand as MemberExpression;
        }

        public static MethodCallExpression AsMethodCallExpressionSkipConvert(this Expression expression)
        {
            var expressionBody = (expression as LambdaExpression)?.Body ?? expression;
            return expressionBody as MethodCallExpression ?? (expressionBody as UnaryExpression)?.Operand as MethodCallExpression;
        }

        public static LambdaExpression AsLambdaExpressionSkipQuote(this Expression expression)
        {
            if (expression.NodeType == ExpressionType.Quote)
            {
                var unaryExpression = (UnaryExpression)expression;
                return unaryExpression.Operand as LambdaExpression;
            }

            return expression as LambdaExpression;
        }

        public static bool IsMemberOfParameter(this Expression expression, ParameterExpression parameterExpression)
        {
            var memberExpression = expression.AsMemberExpressionSkipConvert();
            var checkVisitor = new IsMemberOfParameterExpressionVisitor(parameterExpression);
            checkVisitor.Visit(memberExpression);

            return checkVisitor.Result;
        }

        public static Expression EnsureType(this Expression expression, Type type)
        {
            return expression.Type != type
                ? Expression.Convert(expression, type)
                : expression;
        }

        public static Expression EnsureType<T>(this Expression expression)
        {
            return EnsureType(expression, typeof(T));
        }

        public static MemberInfo GetInitialMemberInfo(this MemberExpression expression)
        {
            var memberExpression = expression.Expression as MemberExpression;
            while (memberExpression != null)
            {
                var nextMemberExpression = memberExpression.Expression as MemberExpression;
                if (nextMemberExpression == null)
                    return memberExpression.Member;

                memberExpression = nextMemberExpression;
            }

            return null;
        }

        public static bool IsMemberOfParameter(this MemberExpression expression, Type type, string name)
        {
            var checkVisitor = new IsMemberOfParameterExpressionVisitor(type, name);
            checkVisitor.Visit(expression);

            return checkVisitor.Result;
        }

        public static bool IsConstant(this MemberExpression memberExpression)
        {
            if (memberExpression.Expression.NodeType == ExpressionType.Constant)
                return true;

            var innerMemberExpression = memberExpression.Expression as MemberExpression;
            return innerMemberExpression?.IsConstant() == true;
        }

        public static string GetMethodName<T>(this Expression<T> self)
        {
            if (self.Body.NodeType != ExpressionType.Call)
                throw new ArgumentException("Must contain method call expression.", nameof(self));

            var callExpressionBody = (MethodCallExpression)self.Body;
            return callExpressionBody.Method.Name;
        }

        public static string GetPropertyName(this Expression selector, bool dontThrowIfNotResolved = false)
        {
            var memberExpression = selector.AsMemberExpressionSkipConvert();
            if (memberExpression == null && !dontThrowIfNotResolved)
                throw new InvalidOperationException("Selector must contain a property access expression.");

            var visitor = new PropertyAccessorVisitor();
            visitor.Visit(selector);

            return visitor.PropertyAccessor;
        }

        public static string GetPropertyName<T>(this Expression<T> selector, bool dontThrowIfNotResolved = false)
        {
            return selector.Body.GetPropertyName(dontThrowIfNotResolved);
        }

        public static string GetPropertyName<TModel, TProperty>(this Expression<Func<TModel, TProperty>> selector, bool dontThrowIfNotResolved = false)
        {
            return selector.GetPropertyName<Func<TModel, TProperty>>(dontThrowIfNotResolved);
        }

        public static IEnumerable<string> GetAccessedMemberNames(this Expression self)
        {
            var memberNameList = new List<string>();

            var lambdaExpression = self as LambdaExpression;
            if (lambdaExpression != null)
            {
                var operandMemberNames = lambdaExpression.Body.GetAccessedMemberNames();
                memberNameList.AddRange(operandMemberNames);
                return memberNameList.Distinct().ToArray();
            }

            var memberExpression = self.AsMemberExpressionSkipConvert();
            if (memberExpression != null)
            {
                var memberName = memberExpression.GetPropertyName();
                memberNameList.Add(memberName);

                return memberNameList.Distinct().ToArray();
            }

            var unaryExpression = self as UnaryExpression;
            if (unaryExpression != null)
            {
                var operandMemberNames = unaryExpression.Operand.GetAccessedMemberNames();
                memberNameList.AddRange(operandMemberNames);
                return memberNameList.Distinct().ToArray();
            }

            var binaryExpression = self as BinaryExpression;
            if (binaryExpression != null)
            {
                var leftMemberNames = binaryExpression.Left.GetAccessedMemberNames();
                var rightMemberNames = binaryExpression.Right.GetAccessedMemberNames();
                memberNameList.AddRange(leftMemberNames);
                memberNameList.AddRange(rightMemberNames);
                return memberNameList.Distinct().ToArray();
            }

            return memberNameList.Distinct().ToArray();
        }

        public static MethodInfo GetMethodInfo<T>(this Expression<T> self)
        {
            if (self.Body.NodeType != ExpressionType.Call)
                throw new ArgumentException("Must contain method call expression.", nameof(self));

            var callExpressionBody = (MethodCallExpression)self.Body;
            return callExpressionBody.Method;
        }

        public static IEnumerable<Expression> GetMethodCallArgumentExpressions<T>(this Expression<T> self)
        {
            if (self.Body.NodeType != ExpressionType.Call)
                throw new ArgumentException("Must contain method call expression.", nameof(self));

            var callExpressionBody = (MethodCallExpression)self.Body;
            return callExpressionBody.Arguments;
        }

        /// <summary>
        /// Evaluate method call arguments.
        /// </summary>
        /// <returns>
        /// Ordered enumeration of method call arguments Type, name and value.
        /// </returns>
        public static IEnumerable<Tuple<Type, string, object>> GetMethodCallArguments<T>(this Expression<T> self)
        {
            if (self.Body.NodeType != ExpressionType.Call)
                throw new ArgumentException("Must contain method call expression.", nameof(self));

            var methodCallExpression = (MethodCallExpression)self.Body;
            var argumentToObjectExpressions = methodCallExpression.Arguments
                .Select(a =>
                {
                    var argumentExpression = Expression.Constant(a.HasParameterExpression()
                        ? a.Type.GetDefaultValue()
                        : a.EvaluateConstExpression());

                    return (Expression)Expression.Convert(argumentExpression, typeof(object));
                })
                .ToArray();

            var newValueArrayExpression = Expression.NewArrayInit(typeof(object), argumentToObjectExpressions);
            var evaluateValueArrayExpression = Expression.Lambda(newValueArrayExpression, self.Parameters);
            var invocationParameters = self.Parameters.Select(p => Activator.CreateInstance(p.Type)).ToArray();
            var values = (object[])evaluateValueArrayExpression.Compile().DynamicInvoke(invocationParameters);

            var methodParameters = GetMethodInfo(self).GetParameters();
            var arguments = methodParameters.Select((p, i) => new Tuple<Type, string, object>(p.ParameterType, p.Name, values[i])).ToArray();

            return arguments;
        }

        public static TExpression RenameParameter<TExpression>(this TExpression self, Func<string, bool> predicate, string name)
            where TExpression : Expression
        {
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            var renamer = new RenameParameterExpressionVisitor(predicate, name);
            var result = (TExpression)renamer.Visit(self);
            return result;
        }

        public static TExpression RenameParameter<TExpression>(this TExpression self, string which, string name)
            where TExpression : Expression
        {
            return self.RenameParameter(p => p == which, name);
        }

        public static TExpression RenameParameter<TExpression>(this TExpression self, string name)
            where TExpression : Expression
        {
            return self.RenameParameter(p => true, name);
        }

        public static TExpression ReplaceMethodCallArgument<TExpression>(this TExpression self, Func<Expression, int, bool> predicate, Expression argument)
            where TExpression : LambdaExpression
        {
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));
            if (argument == null)
                throw new ArgumentNullException(nameof(argument));

            var callExpression = self.AsMethodCallExpressionSkipConvert();
            var arguments = callExpression.Arguments
                .Select((arg, index) =>
                {
                    var isMatch = predicate(arg, index);
                    if (!isMatch)
                        return arg;

                    var isArgumentConvertRequired = arg.Type != argument.Type;
                    return isArgumentConvertRequired
                        ? Expression.Convert(argument, arg.Type)
                        : argument;
                })
                .ToArray();

            var newCallExpression = callExpression.Update(callExpression.Object, arguments);
            var isConvertRequired = self.ReturnType != newCallExpression.Method.ReturnType;
            var newBody = isConvertRequired
                ? (Expression)Expression.Convert(newCallExpression, self.ReturnType)
                : newCallExpression;

            var newLambdaExpression = Expression.Lambda(newBody, self.Name, self.TailCall, self.Parameters);
            return (TExpression)newLambdaExpression;
        }

        public static TExpression ReplaceMethodCallArgument<TExpression>(this TExpression self, Func<Expression, bool> predicate, Expression argument)
            where TExpression : LambdaExpression
        {
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            return self.ReplaceMethodCallArgument((a, i) => predicate(a), argument);
        }

        public static TExpression ReplaceMethodCallArgument<TExpression>(this TExpression self, int index, Expression argument)
            where TExpression : LambdaExpression
        {
            return self.ReplaceMethodCallArgument((a, i) => i == index, argument);
        }

        public static TExpression ReplaceMethodCallArgument<TExpression>(this TExpression self, Expression argument)
            where TExpression : LambdaExpression
        {
            return self.ReplaceMethodCallArgument((a, i) => true, argument);
        }

        public static IEnumerable<Expression> Query(this Expression self, Func<Expression, bool> predicate)
        {
            var queryVisitor = new QueryExpressionVisitor(predicate);
            queryVisitor.Visit(self);

            return queryVisitor.Expressions;
        }

        public static bool HasLinqToObjectsCall(this Expression self)
        {
            var visitor = new LinqToObjectsVisitor();
            visitor.Visit(self);

            return visitor.HasLinqToObjectsCall;
        }

        private class PropertyAccessorVisitor : ExpressionVisitor
        {
            public string PropertyAccessor { get; private set; } = string.Empty;

            public override Expression Visit(Expression node)
            {
                var memberExpression = node.AsMemberExpressionSkipConvert();
                return base.Visit(memberExpression);
            }

            protected override Expression VisitMember(MemberExpression node)
            {
                if (!PropertyAccessor.IsEmpty())
                    PropertyAccessor = $".{PropertyAccessor}";

                PropertyAccessor = node.Member.Name + PropertyAccessor;
                return base.Visit(node.Expression);
            }
        }

        private class ParameterVisitor : ExpressionVisitor
        {
            public bool HasParameterExpression { get; private set; }

            protected override Expression VisitParameter(ParameterExpression node)
            {
                HasParameterExpression = true;
                return base.VisitParameter(node);
            }
        }

        private class IsMemberOfParameterExpressionVisitor : ExpressionVisitor
        {
            private readonly ParameterExpression _parameterExpression;

            public bool Result { get; private set; }

            public IsMemberOfParameterExpressionVisitor(Type type, string name)
            {
                _parameterExpression = Expression.Parameter(type, name);
            }

            public IsMemberOfParameterExpressionVisitor(ParameterExpression parameterExpression)
            {
                if (parameterExpression == null)
                    throw new ArgumentNullException(nameof(parameterExpression));

                _parameterExpression = parameterExpression;
            }

            protected override Expression VisitParameter(ParameterExpression node)
            {
                if (node == _parameterExpression ||
                    (node.Type == _parameterExpression.Type && node.Name == _parameterExpression.Name))
                    Result = true;

                return base.VisitParameter(node);
            }
        }

        private class RenameParameterExpressionVisitor : ExpressionVisitor
        {
            private readonly Func<string, bool> _predicate;
            private readonly string _name;

            public RenameParameterExpressionVisitor(Func<string, bool> predicate, string name)
            {
                if (predicate == null)
                    throw new ArgumentNullException(nameof(predicate));
                if (name.IsNullOrEmpty())
                    throw new ArgumentException("Must be non-empty string.", nameof(name));

                _predicate = predicate;
                _name = name;
            }

            protected override Expression VisitParameter(ParameterExpression node)
            {
                var isMatch = _predicate(node.Name);
                return isMatch ? Expression.Parameter(node.Type, _name) : node;
            }
        }

        private class LinqToObjectsVisitor : ExpressionVisitor
        {
            public bool HasLinqToObjectsCall { get; private set; }

            protected override Expression VisitMethodCall(MethodCallExpression node)
            {
                if (node.Method.IsLinqToObjectsMethod())
                    HasLinqToObjectsCall = true;

                return base.VisitMethodCall(node);
            }
        }

        private class EvaluateStaticValuesToConstantsVisitor : ExpressionVisitor
        {
            public override Expression Visit(Expression node)
            {
                var canEvaluate = node is ConstantExpression == false && node is DynamicExpression == false && !node.HasParameterExpression();
                if (canEvaluate)
                {
                    var value = node.EvaluateConstExpression();
                    var constantNode = Expression.Constant(value);
                    return base.Visit(constantNode);
                }

                return base.Visit(node);
            }

            protected override Expression VisitMemberInit(MemberInitExpression node)
            {
                if (node.NewExpression.NodeType == ExpressionType.New)
                    return node;

                return base.VisitMemberInit(node);
            }
        }

        private class QueryExpressionVisitor : ExpressionVisitor
        {
            public IEnumerable<Expression> Expressions => _expressions.AsReadOnly();

            private readonly Func<Expression, bool> _predicate;
            private readonly List<Expression> _expressions;

            public QueryExpressionVisitor(Func<Expression, bool> predicate)
            {
                _predicate = predicate ?? throw new ArgumentNullException(nameof(predicate));
                _expressions = new List<Expression>();
            }

            public override Expression Visit(Expression node)
            {
                if (_predicate(node))
                    _expressions.Add(node);

                return base.Visit(node);
            }
        }
    }
}