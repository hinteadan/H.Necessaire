using Google.Cloud.Firestore;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace H.Necessaire.Runtime.Google.FirestoreDB.Core.Model.Querying
{
    public class HsFirestoreFilterCriteria : ImAFirestoreCriteria
    {
        public FieldPath Property { get; set; }
        public HsFirestoreFilterOperator Operator { get; set; }
        public object Value { get; set; }

        public Filter ToFilter()
        {
            switch (Operator)
            {
                case HsFirestoreFilterOperator.ContainsAll:
                    return BuildContainsFilter(HsFirestoreCompositionOperator.And);
                case HsFirestoreFilterOperator.ContainsAny:
                    return BuildContainsFilter(HsFirestoreCompositionOperator.Or);

                case HsFirestoreFilterOperator.NotEqualToCaseInsensitive:
                    return Filter.NotEqualTo(Property, (Value as string)?.ToLowerInvariant());

                case HsFirestoreFilterOperator.EqualToCaseInsensitive:
                    return Filter.EqualTo(Property, (Value as string)?.ToLowerInvariant());

                case HsFirestoreFilterOperator.LessThan:
                    return Filter.LessThan(Property, Value);
                case HsFirestoreFilterOperator.LessThanOrEqualTo:
                    return Filter.LessThanOrEqualTo(Property, Value);
                case HsFirestoreFilterOperator.GreaterThan:
                    return Filter.GreaterThan(Property, Value);
                case HsFirestoreFilterOperator.GreaterThanOrEqualTo:
                    return Filter.GreaterThanOrEqualTo(Property, Value);

                case HsFirestoreFilterOperator.ArrayContains:
                    return Filter.ArrayContains(Property, Value);
                case HsFirestoreFilterOperator.ArrayContainsAny:
                    return Filter.ArrayContainsAny(Property, Value as IEnumerable);

                case HsFirestoreFilterOperator.InArray:
                    return Filter.InArray(Property, Value as IEnumerable);
                case HsFirestoreFilterOperator.NotInArray:
                    return Filter.NotInArray(Property, Value as IEnumerable);


                
                case HsFirestoreFilterOperator.NotEqualTo:
                    return Filter.NotEqualTo(Property, Value);

                case HsFirestoreFilterOperator.EqualTo:
                default:
                    return Filter.EqualTo(Property, Value);
            }
        }

        private Filter BuildContainsFilter(HsFirestoreCompositionOperator @operator)
        {
            return
                new HsFirestoreCompositionCriteria
                {
                    Operator = @operator,
                    Criterias
                        = EnsureValueAsStringEnumerableOrNull(Value)
                        ?.Select(key => key.NullIfEmpty()?.ToLowerInvariant())
                        .ToNoNullsArray()
                        ?.TrimToValidKeywordsOnly(minLength: 3, maxNumberOfKeywords: 30)
                        ?.Select(key => new HsFirestoreFilterCriteria
                        {
                            Property = Property,
                            Operator = HsFirestoreFilterOperator.ArrayContains,
                            Value = key,
                        })
                        .ToArray()

                }
                .ToFilter();
        }

        private IEnumerable<string> EnsureValueAsStringEnumerableOrNull(object value)
        {
            if (value is null)
                return null;

            if(value is IEnumerable<string>)
                return value as IEnumerable<string>;

            if (value is string)
                return (value as string).AsArray();

            return null;
        }
    }
}
