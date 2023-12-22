using System;
using System.Runtime.Serialization;

namespace H.Necessaire.Runtime.Azure.CosmosDB.Core.Model
{
    [DataContract]
    internal class HsCosmosItem<TData> : IStringIdentity
    {
        [DataMember(Name = "id")] public virtual string ID { get; set; } = Guid.NewGuid().ToString();
        [DataMember(Name = "partitionKey")] public virtual string PartitionKey { get; set; } = typeof(TData).ToPartitionKey();
        [DataMember(Name = "dataType")] public virtual string DataType { get; set; } = typeof(TData).TypeName();
        [DataMember(Name = "data")] public virtual TData Data { get; set; }
    }
}
