using H.Necessaire.Runtime.Azure.CosmosDB.Core.Model;
using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Linq;

namespace H.Necessaire.Runtime.Azure.CosmosDB.Core
{
    internal static class HsCosmosExtensions
    {
        public static HsCosmosItem<T> ToCosmosItem<T>(this T data, string id = null, string partitionKey = null, string dataType = null)
        {
            return
                new HsCosmosItem<T>
                {
                    Data = data,
                }
                .And(x =>
                {
                    if (!id.IsEmpty())
                    {
                        x.ID = id;
                    }
                    else if (data != null)
                    {
                        x.ID =
                            (data as IGuidIdentity)?.ID.ToString()
                            ?? (data as IStringIdentity)?.ID.NullIfEmpty()
                            ?? x.ID
                            ;
                    }

                    if (!partitionKey.IsEmpty())
                        x.PartitionKey = partitionKey;
                    if (!dataType.IsEmpty())
                        x.DataType = dataType;
                })
                ;
        }

        public static QueryDefinition WithParameters(this QueryDefinition query, IDictionary<string, object> sqlParams)
        {
            if (query == null)
                return query;

            if (sqlParams?.Any() != true)
                return query;

            foreach (KeyValuePair<string, object> p in sqlParams)
            {
                query = query.WithParameter(EnsureParamName(p.Key), p.Value);
            }

            return query;
        }

        public static string ToPartitionKey(this Type type)
            => type?.Name ?? Guid.NewGuid().ToString();

        private static string EnsureParamName(string name)
        {
            if (name.IsEmpty())
                return name;

            if (name.StartsWith("@"))
                return name;

            return $"@{name}";
        }
    }
}
