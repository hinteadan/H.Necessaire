using Google.Cloud.Firestore;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace H.Necessaire.Runtime.Google.FirestoreDB.Core.Model.Querying
{
    public static class HsFirestoreQueryExtensions
    {
        public static string[] Parts(this string path, bool hasRootVar = false)
        {
            if (path.IsEmpty())
                return null;

            string[] segments
                = (hasRootVar ? path : $"x.{path}")
                .Split(".")
                .And(x =>
                {
                    x[0] = nameof(HsFirestoreDocument.Data);
                })
                ;

            return segments;
        }
        public static string[] PartsForLoCase(this string path)
            => path.Parts()?.And(x => x[x.Length - 1] = $"{x[x.Length - 1]}AsLowerCase");
        public static string[] PartsForKeys(this string path)
            => path.Parts()?.And(x => x[x.Length - 1] = $"{x[x.Length - 1]}AsLowerCaseKeys");
        public static FieldPath Path(this string[] parts)
            => parts?.Any() != true ? null : new FieldPath(parts);
        public static string[] Parts<T, U>(this T data, Expression<Func<T, U>> propertySelector)
            => (propertySelector?.Body?.ToString()).Parts(hasRootVar: true);
        public static FieldPath Path<T, U>(this T data, Expression<Func<T, U>> propertySelector)
            => data?.Parts(propertySelector)?.Path();
        public static FieldPath PathForLoCase<T, U>(this T data, Expression<Func<T, U>> propertySelector)
            => data?.Parts(propertySelector)?.And(x => x[x.Length - 1] = $"{x[x.Length - 1]}AsLowerCase")?.Path();
        public static FieldPath PathForKeys<T, U>(this T data, Expression<Func<T, U>> propertySelector)
            => data?.Parts(propertySelector)?.And(x => x[x.Length - 1] = $"{x[x.Length - 1]}AsLowerCaseKeys")?.Path();

        public static HsFirestoreFilterCriteria Filter(this string path, HsFirestoreFilterOperator @operator, object value)
        {
            FieldPath fieldPath = null;
            if (@operator.In(HsFirestoreFilterOperator.ContainsAny, HsFirestoreFilterOperator.ContainsAll))
                fieldPath = path?.PartsForKeys()?.Path();
            else if (@operator.In(HsFirestoreFilterOperator.EqualToCaseInsensitive, HsFirestoreFilterOperator.NotEqualToCaseInsensitive))
                fieldPath = path?.PartsForLoCase()?.Path();
            else
                fieldPath = path?.Parts()?.Path();

            if (fieldPath == null)
                return null;

            return
                new HsFirestoreFilterCriteria
                {
                    Property = fieldPath,
                    Operator = @operator,
                    Value = value,
                };
        }
        public static HsFirestoreFilterCriteria Filter(this string path, string @operator, object value)
            => path?.Filter(@operator.Op(), value);

        public static HsFirestoreFilterCriteria Filter<T, U>(this T data, Expression<Func<T, U>> propertySelector, HsFirestoreFilterOperator @operator, object value)
        {
            FieldPath fieldPath = null;
            if (@operator.In(HsFirestoreFilterOperator.ContainsAny, HsFirestoreFilterOperator.ContainsAll))
                fieldPath = data?.PathForKeys(propertySelector);
            else if (@operator.In(HsFirestoreFilterOperator.EqualToCaseInsensitive, HsFirestoreFilterOperator.NotEqualToCaseInsensitive))
                fieldPath = data?.PathForLoCase(propertySelector);
            else
                fieldPath = data?.Path(propertySelector);

            if (fieldPath == null)
                return null;

            return
                new HsFirestoreFilterCriteria
                {
                    Property = fieldPath,
                    Operator = @operator,
                    Value = value,
                };
        }
        public static HsFirestoreFilterCriteria Filter<T, U>(this T data, Expression<Func<T, U>> propertySelector, string @operator, object value)
            => data?.Filter(propertySelector, @operator.Op(), value);
        public static HsFirestoreFilterOperator Op(this string op)
        {
            switch (op?.ToLowerInvariant())
            {
                case "any": return HsFirestoreFilterOperator.ContainsAny;
                case "all": return HsFirestoreFilterOperator.ContainsAll;

                case "*<>":
                case "*!=": return HsFirestoreFilterOperator.NotEqualToCaseInsensitive;
                case "*=":
                case "*==": return HsFirestoreFilterOperator.EqualToCaseInsensitive;

                case "<": return HsFirestoreFilterOperator.LessThan;
                case "<=": return HsFirestoreFilterOperator.LessThanOrEqualTo;
                case ">": return HsFirestoreFilterOperator.GreaterThan;
                case ">=": return HsFirestoreFilterOperator.GreaterThanOrEqualTo;

                case "[?]": return HsFirestoreFilterOperator.ArrayContains;
                case "[*]": return HsFirestoreFilterOperator.ArrayContainsAny;

                case "[=]":
                case "[==]": return HsFirestoreFilterOperator.InArray;
                case "[<>]":
                case "[!=]": return HsFirestoreFilterOperator.NotInArray;

                case "<>":
                case "!=": return HsFirestoreFilterOperator.NotEqualTo;
                case "==":
                case "=":
                default: return HsFirestoreFilterOperator.EqualTo;
            }
        }
    }
}
