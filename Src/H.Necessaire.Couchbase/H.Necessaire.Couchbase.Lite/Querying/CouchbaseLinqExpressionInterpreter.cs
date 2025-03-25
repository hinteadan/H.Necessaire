using Couchbase.Lite.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using CouchbaseExpression = Couchbase.Lite.Query.Expression;
using Expression = System.Linq.Expressions.Expression;

namespace H.Necessaire.Couchbase.Lite.Querying
{
    class CouchbaseLinqExpressionInterpreter
    {
        public static readonly CouchbaseLinqExpressionInterpreter Instance = new CouchbaseLinqExpressionInterpreter();

        public ISelectResult[] SelectAll() => new ISelectResult[] { SelectResult.All() };

        public ISelectResult[] SelectCount() => new ISelectResult[] { SelectResult.Expression(Function.Count(CouchbaseExpression.All())).As("Count") };

        public ISelectResult[] Select<T>(params Expression<Func<T, object>>[] selectors)
        {
            IExpression[] propExpressions = selectors?.Select(x => BuildPropertySelectorFromExpression(x)).ToNoNullsArray();

            if (propExpressions.IsEmpty())
                return null;

            return
                propExpressions
                .Select(x => SelectResult.Expression(x))
                .ToArray()
                ;
        }

        public IExpression Where<T>(Expression<Func<T, bool>> filter) => BuildWhereFromExpression(filter);
        public IExpression GroupBy<T>(Expression<Func<T, object>> selector)
            => BuildCouchbaseExpressionFromLinqExpression(selector);
        public IOrdering OrderBy<T>(Expression<Func<T, object>> selector, bool isDesc = false)
            => isDesc
            ? Ordering.Expression(BuildCouchbaseExpressionFromLinqExpression(selector)).Descending()
            : Ordering.Expression(BuildCouchbaseExpressionFromLinqExpression(selector)).Ascending()
            ;
        public IExpression Limit<T>(Expression<Func<T, long>> limit)
            => BuildCouchbaseExpressionFromLinqExpression(limit);
        public IExpression Limit<T>(long limit) => CouchbaseExpression.Long(limit);
        public IExpression Offset<T>(Expression<Func<T, long>> offset = null)
            => offset is null ? null : BuildCouchbaseExpressionFromLinqExpression(offset);
        public IExpression Offset<T>(long? offset = null)
            => offset is null ? null : CouchbaseExpression.Long(offset.Value);

        static IExpression BuildPropertySelectorFromExpression<T>(Expression<Func<T, object>> propertySelector)
            => BuildCouchbaseExpressionFromLinqExpression(propertySelector.Body);

        static IExpression BuildWhereFromExpression<T>(Expression<Func<T, bool>> expression)
            => BuildCouchbaseExpressionFromLinqExpression(expression.Body);

        static IExpression BuildCouchbaseExpressionFromLinqExpression(Expression expression)
        {
            switch (expression.NodeType)
            {
                case ExpressionType.Constant:
                    ConstantExpression constantExpression = expression as ConstantExpression;
                    return BuildCouchbaseConstantExpression(constantExpression);

                case ExpressionType.Call:
                    MethodCallExpression methodCallExpression = expression as MethodCallExpression;
                    return BuildCouchbaseMethodCallExpression(methodCallExpression);

                case ExpressionType.MemberAccess:
                    MemberExpression memberAccessExpression = expression as MemberExpression;
                    return BuildCouchbaseMemberAccessExpression(memberAccessExpression);

                case ExpressionType.Add:
                case ExpressionType.AddChecked:
                    BinaryExpression binaryExpression = expression as BinaryExpression;
                    return BuildCouchbaseExpressionFromLinqExpression(binaryExpression.Left).Add(BuildCouchbaseExpressionFromLinqExpression(binaryExpression.Right));

                case ExpressionType.Subtract:
                case ExpressionType.SubtractChecked:
                    binaryExpression = expression as BinaryExpression;
                    return BuildCouchbaseExpressionFromLinqExpression(binaryExpression.Left).Subtract(BuildCouchbaseExpressionFromLinqExpression(binaryExpression.Right));

                case ExpressionType.And:
                case ExpressionType.AndAlso:
                    binaryExpression = expression as BinaryExpression;
                    return BuildCouchbaseExpressionFromLinqExpression(binaryExpression.Left).And(BuildCouchbaseExpressionFromLinqExpression(binaryExpression.Right));

                case ExpressionType.Or:
                case ExpressionType.OrElse:
                    binaryExpression = expression as BinaryExpression;
                    return BuildCouchbaseExpressionFromLinqExpression(binaryExpression.Left).Or(BuildCouchbaseExpressionFromLinqExpression(binaryExpression.Right));

                case ExpressionType.ArrayLength:
                    UnaryExpression unaryExpression = expression as UnaryExpression;
                    return ArrayFunction.Length(BuildCouchbaseExpressionFromLinqExpression(unaryExpression.Operand));

                case ExpressionType.Convert:
                case ExpressionType.ConvertChecked:
                    unaryExpression = expression as UnaryExpression;
                    return BuildCouchbaseExpressionFromLinqExpression(unaryExpression.Operand);

                case ExpressionType.Divide:
                    binaryExpression = expression as BinaryExpression;
                    return BuildCouchbaseExpressionFromLinqExpression(binaryExpression.Left).Divide(BuildCouchbaseExpressionFromLinqExpression(binaryExpression.Right));

                case ExpressionType.Equal:
                    binaryExpression = expression as BinaryExpression;
                    return BuildCouchbaseExpressionFromLinqExpression(binaryExpression.Left).EqualTo(BuildCouchbaseExpressionFromLinqExpression(binaryExpression.Right));

                case ExpressionType.NotEqual:
                    binaryExpression = expression as BinaryExpression;
                    return BuildCouchbaseExpressionFromLinqExpression(binaryExpression.Left).NotEqualTo(BuildCouchbaseExpressionFromLinqExpression(binaryExpression.Right));

                case ExpressionType.GreaterThan:
                    binaryExpression = expression as BinaryExpression;
                    return BuildCouchbaseExpressionFromLinqExpression(binaryExpression.Left).GreaterThan(BuildCouchbaseExpressionFromLinqExpression(binaryExpression.Right));

                case ExpressionType.GreaterThanOrEqual:
                    binaryExpression = expression as BinaryExpression;
                    return BuildCouchbaseExpressionFromLinqExpression(binaryExpression.Left).GreaterThanOrEqualTo(BuildCouchbaseExpressionFromLinqExpression(binaryExpression.Right));

                case ExpressionType.LessThan:
                    binaryExpression = expression as BinaryExpression;
                    return BuildCouchbaseExpressionFromLinqExpression(binaryExpression.Left).LessThan(BuildCouchbaseExpressionFromLinqExpression(binaryExpression.Right));

                case ExpressionType.LessThanOrEqual:
                    binaryExpression = expression as BinaryExpression;
                    return BuildCouchbaseExpressionFromLinqExpression(binaryExpression.Left).LessThanOrEqualTo(BuildCouchbaseExpressionFromLinqExpression(binaryExpression.Right));

                case ExpressionType.Modulo:
                    binaryExpression = expression as BinaryExpression;
                    return BuildCouchbaseExpressionFromLinqExpression(binaryExpression.Left).Modulo(BuildCouchbaseExpressionFromLinqExpression(binaryExpression.Right));

                case ExpressionType.Multiply:
                case ExpressionType.MultiplyChecked:
                    binaryExpression = expression as BinaryExpression;
                    return BuildCouchbaseExpressionFromLinqExpression(binaryExpression.Left).Multiply(BuildCouchbaseExpressionFromLinqExpression(binaryExpression.Right));

                case ExpressionType.Power:
                    binaryExpression = expression as BinaryExpression;
                    return Function.Power(BuildCouchbaseExpressionFromLinqExpression(binaryExpression.Left), BuildCouchbaseExpressionFromLinqExpression(binaryExpression.Right));

                case ExpressionType.Negate:
                case ExpressionType.NegateChecked:
                    return CouchbaseExpression.Int(-1).Multiply(BuildCouchbaseExpressionFromLinqExpression((expression as UnaryExpression).Operand));

                case ExpressionType.Increment:
                    return BuildCouchbaseExpressionFromLinqExpression((expression as UnaryExpression).Operand).Add(CouchbaseExpression.Int(1));

                case ExpressionType.Decrement:
                    return BuildCouchbaseExpressionFromLinqExpression((expression as UnaryExpression).Operand).Subtract(CouchbaseExpression.Int(1));

                case ExpressionType.Not:
                    return CouchbaseExpression.Not(BuildCouchbaseExpressionFromLinqExpression((expression as UnaryExpression).Operand));

                case ExpressionType.IsTrue:
                    return BuildCouchbaseExpressionFromLinqExpression((expression as UnaryExpression).Operand).EqualTo(CouchbaseExpression.Boolean(true));

                case ExpressionType.IsFalse:
                    return BuildCouchbaseExpressionFromLinqExpression((expression as UnaryExpression).Operand).EqualTo(CouchbaseExpression.Boolean(false));

                case ExpressionType.Lambda:
                    return BuildCouchbaseExpressionFromLinqExpression((expression as LambdaExpression).Body);

                case ExpressionType.Invoke:
                    InvocationExpression invocationExpression = expression as InvocationExpression;
                    return BuildCouchbaseExpressionFromLinqExpression(invocationExpression.Expression);

                case ExpressionType.Quote:
                    return BuildCouchbaseExpressionFromLinqExpression((expression as UnaryExpression).Operand);

                case ExpressionType.Parameter:
                    ParameterExpression parameterExpression = expression as ParameterExpression;
                    return CouchbaseExpression.Parameter(parameterExpression.Name);

                case ExpressionType.ArrayIndex:
                case ExpressionType.Coalesce:
                case ExpressionType.Conditional:
                case ExpressionType.ExclusiveOr:
                case ExpressionType.LeftShift:
                case ExpressionType.ListInit:
                case ExpressionType.MemberInit:
                case ExpressionType.UnaryPlus:
                case ExpressionType.New:
                case ExpressionType.NewArrayInit:
                case ExpressionType.NewArrayBounds:
                case ExpressionType.RightShift:
                case ExpressionType.TypeAs:
                case ExpressionType.TypeIs:
                case ExpressionType.Assign:
                case ExpressionType.Block:
                case ExpressionType.DebugInfo:
                case ExpressionType.Dynamic:
                case ExpressionType.Default:
                case ExpressionType.Extension:
                case ExpressionType.Goto:
                case ExpressionType.Index:
                case ExpressionType.Label:
                case ExpressionType.RuntimeVariables:
                case ExpressionType.Loop:
                case ExpressionType.Switch:
                case ExpressionType.Throw:
                case ExpressionType.Try:
                case ExpressionType.Unbox:
                case ExpressionType.AddAssign:
                case ExpressionType.AndAssign:
                case ExpressionType.DivideAssign:
                case ExpressionType.ExclusiveOrAssign:
                case ExpressionType.LeftShiftAssign:
                case ExpressionType.ModuloAssign:
                case ExpressionType.MultiplyAssign:
                case ExpressionType.OrAssign:
                case ExpressionType.PowerAssign:
                case ExpressionType.RightShiftAssign:
                case ExpressionType.SubtractAssign:
                case ExpressionType.AddAssignChecked:
                case ExpressionType.MultiplyAssignChecked:
                case ExpressionType.SubtractAssignChecked:
                case ExpressionType.PreIncrementAssign:
                case ExpressionType.PreDecrementAssign:
                case ExpressionType.PostIncrementAssign:
                case ExpressionType.PostDecrementAssign:
                case ExpressionType.TypeEqual:
                case ExpressionType.OnesComplement:

                default:
                    throw new NotSupportedException($"Expression {expression.NodeType} is not supported");
            }
        }

        static IExpression BuildCouchbaseMemberAccessExpression(MemberExpression memberExpression)
        {
            if (memberExpression.Expression is null)
                return CouchbaseExpression.Value(ReadMemberValue(memberExpression));

            switch (memberExpression.Expression.NodeType)
            {
                case ExpressionType.Constant:
                    return CouchbaseExpression.Value(ReadMemberValue(memberExpression, (memberExpression.Expression as ConstantExpression).Value));
                case ExpressionType.Parameter:
                default:
                    return CouchbaseExpression.Property(BuildPropertyPathFromExpression(memberExpression));
            }
        }

        static string BuildPropertyPathFromExpression(Expression expression, string separator = ".")
        {
            MemberExpression memberExpression;
            switch (expression.NodeType)
            {
                case ExpressionType.Convert:
                case ExpressionType.ConvertChecked:
                    var unaryExpression = expression as UnaryExpression;
                    memberExpression = (unaryExpression != null ? unaryExpression.Operand : null) as MemberExpression;
                    break;
                default:
                    memberExpression = expression as MemberExpression;
                    break;
            }

            if (memberExpression is null)
                return null;

            Stack<string> partsStack = new Stack<string>(20);
            partsStack.Push(memberExpression.Member.Name);

            memberExpression = memberExpression.Expression as MemberExpression;

            while (memberExpression != null)
            {
                partsStack.Push(memberExpression.Member.Name);
                memberExpression = memberExpression.Expression as MemberExpression;
            }

            return string.Join(separator, partsStack);
        }

        static IExpression BuildCouchbaseConstantExpression(ConstantExpression constantExpression)
        {
            if (constantExpression.Value is double doubleValue)
            {
                if (doubleValue == Math.E)
                    return Function.E();
                if (doubleValue == Math.PI)
                    return Function.Pi();
            }

            return CouchbaseExpression.Value(constantExpression.Value);
        }

        static IExpression BuildCouchbaseMethodCallExpression(MethodCallExpression methodExpression)
        {
            string methodName = methodExpression.Method.Name;
            Type declaringType = methodExpression.Method.DeclaringType;
            Expression[] methodArgs = methodExpression.Arguments.ToArray();
            Expression target = methodExpression.Object;
            switch (methodName)
            {
                case nameof(Math.Abs):
                    return Function.Abs(BuildCouchbaseExpressionFromLinqExpression(methodArgs.Single()));
                case nameof(Math.Acos):
                    return Function.Acos(BuildCouchbaseExpressionFromLinqExpression(methodArgs.Single()));
                case nameof(Math.Asin):
                    return Function.Asin(BuildCouchbaseExpressionFromLinqExpression(methodArgs.Single()));
                case nameof(Math.Atan):
                    return Function.Atan(BuildCouchbaseExpressionFromLinqExpression(methodArgs.Single()));
                case nameof(Math.Atan2):
                    return Function.Atan2(BuildCouchbaseExpressionFromLinqExpression(methodArgs[0]), BuildCouchbaseExpressionFromLinqExpression(methodArgs[1]));
                case "Average":
                case "Avg":
                    return Function.Avg(BuildCouchbaseExpressionFromLinqExpression(methodArgs.Single()));
                case nameof(Math.Ceiling):
                case "Ceil":
                    return Function.Ceil(BuildCouchbaseExpressionFromLinqExpression(methodArgs.Single()));
                case nameof(string.Contains):
                    return declaringType == typeof(string)
                        ? Function.Contains(BuildCouchbaseExpressionFromLinqExpression(target), BuildCouchbaseExpressionFromLinqExpression(methodArgs.Single()))
                        : ArrayFunction.Contains(BuildCouchbaseExpressionFromLinqExpression(methodArgs[0]), BuildCouchbaseExpressionFromLinqExpression(methodArgs[1]))
                        ;
                case nameof(Math.Cos):
                    return Function.Cos(BuildCouchbaseExpressionFromLinqExpression(methodArgs.Single()));
                case "Count":
                    return Function.Count(methodArgs.IsEmpty() ? CouchbaseExpression.All() : BuildCouchbaseExpressionFromLinqExpression(methodArgs.Single()));
                case "Degrees":
                case "RadiansToDegrees":
                case "RadToDeg":
                case "RadDeg":
                    return Function.Degrees(BuildCouchbaseExpressionFromLinqExpression(methodArgs.Single()));
                case nameof(Math.Exp):
                    return Function.Exp(BuildCouchbaseExpressionFromLinqExpression(methodArgs.Single()));
                case nameof(Math.Floor):
                    return Function.Floor(BuildCouchbaseExpressionFromLinqExpression(methodArgs.Single()));
                case nameof(string.Length):
                    return Function.Length(BuildCouchbaseExpressionFromLinqExpression(methodArgs.Single()));
                case nameof(Math.Log):
                    return Function.Ln(BuildCouchbaseExpressionFromLinqExpression(methodArgs.Single()));
                case nameof(Math.Log10):
                    return Function.Log(BuildCouchbaseExpressionFromLinqExpression(methodArgs.Single()));
                case nameof(string.ToLower):
                case nameof(string.ToLowerInvariant):
                    return Function.Lower(BuildCouchbaseExpressionFromLinqExpression(methodArgs.Single()));
                case nameof(string.TrimStart):
                    return Function.Ltrim(BuildCouchbaseExpressionFromLinqExpression(methodArgs.Single()));
                case nameof(Math.Max):
                    return Function.Max(BuildCouchbaseExpressionFromLinqExpression(methodArgs.Single()));
                case "MillisToString":
                case "MsToString":
                    return Function.MillisToString(BuildCouchbaseExpressionFromLinqExpression(methodArgs.Single()));
                case "MillisToUTC":
                case "MsToUTC":
                    return Function.MillisToUTC(BuildCouchbaseExpressionFromLinqExpression(methodArgs.Single()));
                case nameof(Math.Min):
                    return Function.Min(BuildCouchbaseExpressionFromLinqExpression(methodArgs.Single()));
                case nameof(Math.Pow):
                    return Function.Power(BuildCouchbaseExpressionFromLinqExpression(methodArgs[0]), BuildCouchbaseExpressionFromLinqExpression(methodArgs[1]));
                case "Radians":
                case "DegreesToRadians":
                case "DegToRad":
                case "DegRad":
                    return Function.Radians(BuildCouchbaseExpressionFromLinqExpression(methodArgs.Single()));
                case nameof(Math.Round):
                    return methodArgs.Length == 1
                        ? Function.Round(BuildCouchbaseExpressionFromLinqExpression(methodArgs.Single()))
                        : Function.Round(BuildCouchbaseExpressionFromLinqExpression(methodArgs[0]), BuildCouchbaseExpressionFromLinqExpression(methodArgs[1]))
                        ;
                case nameof(Math.Sign):
                    return Function.Sign(BuildCouchbaseExpressionFromLinqExpression(methodArgs.Single()));
                case nameof(Math.Sin):
                    return Function.Sin(BuildCouchbaseExpressionFromLinqExpression(methodArgs.Single()));
                case nameof(Math.Sqrt):
                    return Function.Sqrt(BuildCouchbaseExpressionFromLinqExpression(methodArgs.Single()));
                case "StringToMillis":
                case "StringToMs":
                    return Function.StringToMillis(BuildCouchbaseExpressionFromLinqExpression(methodArgs.Single()));
                case "StringToUTC":
                    return Function.StringToUTC(BuildCouchbaseExpressionFromLinqExpression(methodArgs.Single()));
                case "Sum":
                    return Function.Sum(BuildCouchbaseExpressionFromLinqExpression(methodArgs.Single()));
                case nameof(Math.Tan):
                    return Function.Tan(BuildCouchbaseExpressionFromLinqExpression(methodArgs.Single()));
                case nameof(string.Trim):
                    return Function.Trim(BuildCouchbaseExpressionFromLinqExpression(methodArgs.Single()));
                case "Trunc":
                case nameof(decimal.Truncate):
                    return methodArgs.Length == 1
                        ? Function.Trunc(BuildCouchbaseExpressionFromLinqExpression(methodArgs.Single()))
                        : Function.Trunc(BuildCouchbaseExpressionFromLinqExpression(methodArgs[0]), BuildCouchbaseExpressionFromLinqExpression(methodArgs[1]))
                        ;
                case nameof(string.ToUpper):
                case nameof(string.ToUpperInvariant):
                    return Function.Upper(BuildCouchbaseExpressionFromLinqExpression(methodArgs.Single()));

                case nameof(CollectionExtensions.In):
                    return ArrayFunction.Contains(BuildCouchbaseExpressionFromLinqExpression(methodArgs[1]), BuildCouchbaseExpressionFromLinqExpression(methodArgs[0]));

                case nameof(CollectionExtensions.NotIn):
                    return CouchbaseExpression.Not(ArrayFunction.Contains(BuildCouchbaseExpressionFromLinqExpression(methodArgs[1]), BuildCouchbaseExpressionFromLinqExpression(methodArgs[0])));

                case "Like":
                    return Function.Lower(BuildCouchbaseExpressionFromLinqExpression(methodArgs[0])).Like(Function.Lower(BuildCouchbaseExpressionFromLinqExpression(methodArgs[1])));

                case "Regex":
                    return BuildCouchbaseExpressionFromLinqExpression(methodArgs[0]).Regex(BuildCouchbaseExpressionFromLinqExpression(methodArgs[1]));

                default:
                    return CouchbaseExpression.Value(Expression.Lambda(methodExpression).Compile().DynamicInvoke());
            }
        }

        static object ReadMemberValue(MemberExpression memberExpression, object owner = null)
        {
            return
                memberExpression.Member.MemberType == MemberTypes.Property
                ? (memberExpression.Member as PropertyInfo).GetValue(owner)
                : memberExpression.Member.MemberType == MemberTypes.Field
                ? (memberExpression.Member as FieldInfo).GetValue(owner)
                : throw new NotSupportedException($"Member type {memberExpression.Member.MemberType} not supported")
                ;
        }
    }
}
