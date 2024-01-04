using Google.Cloud.Firestore;
using System.Linq;

namespace H.Necessaire.Runtime.Google.FirestoreDB.Core.Model.Querying
{
    public class HsFirestoreCompositionCriteria : ImAFirestoreCriteria
    {
        public ImAFirestoreCriteria[] Criterias { get; set; }
        public HsFirestoreCompositionOperator Operator { get; set; }

        public Filter ToFilter()
        {
            Filter[] validFilters = Criterias?.Select(x => x?.ToFilter()).ToNoNullsArray();
            if (validFilters?.Any() != true)
                return null;

            if (validFilters.Length == 1)
                return validFilters.Single();

            return
                Operator == HsFirestoreCompositionOperator.And
                ? Filter.And(validFilters)
                : Filter.Or(validFilters)
                ;
        }
    }
}
