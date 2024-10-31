using Google.Cloud.Firestore;
using System;

namespace H.Necessaire.Runtime.Google.FirestoreDB.Core.Model
{
    [FirestoreData]

    public class HsFirestoreDocument : IStringIdentity
    {
        [FirestoreDocumentId] public string ID { get; set; }
        [FirestoreDocumentCreateTimestamp] public DateTime? CreatedAt { get; set; }
        [FirestoreDocumentUpdateTimestamp] public DateTime? LastUpdatedAt { get; set; }
        [FirestoreDocumentReadTimestamp] public DateTime? ReadAt { get; set; }
        [FirestoreProperty] public string DataType { get; set; }
        [FirestoreProperty] public string DataTypeShortName { get; set; }
        [FirestoreProperty] public object Data { get; set; }
    }
}
