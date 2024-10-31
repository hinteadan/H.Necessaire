namespace H.Necessaire.Runtime.Google.FirestoreDB.Core
{
    public class HsFirestoreConfig
    {
        public const string DefaultCollectionName = "HNecessaireDefault";

        public string ProjectID { get; set; }
        public string CollectionName { get; set; } = DefaultCollectionName;
    }
}
