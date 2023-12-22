using H.Necessaire.Serialization;
using Microsoft.Azure.Cosmos;
using System.IO;

namespace H.Necessaire.Runtime.Azure.CosmosDB.Core
{
    internal class HsCosmosSerializer : CosmosSerializer
    {
        public static readonly HsCosmosSerializer Instance = new HsCosmosSerializer();

        public override T FromStream<T>(Stream stream)
        {
            string jsonString = stream.ReadAsStringAsync(isStreamLeftOpen: false).ConfigureAwait(continueOnCapturedContext: false).GetAwaiter().GetResult();
            return jsonString.JsonToObject<T>();
        }

        public override Stream ToStream<T>(T input)
        {
            string jsonString = input.ToJsonObject();
            return jsonString.ToStream();
        }
    }
}
