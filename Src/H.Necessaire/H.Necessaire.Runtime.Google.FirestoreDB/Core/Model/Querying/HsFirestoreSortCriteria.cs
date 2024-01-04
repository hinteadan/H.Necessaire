using Google.Cloud.Firestore;

namespace H.Necessaire.Runtime.Google.FirestoreDB.Core.Model.Querying
{
    public class HsFirestoreSortCriteria
    {
        public FieldPath Property { get; set; }
        public SortDirection Direction { get; set; }

        public enum SortDirection
        {
            Descending = -1,
            Ascending = 0,
        }
    }
}
