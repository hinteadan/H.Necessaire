using Microsoft.Azure.Cosmos;
using System;

namespace H.Necessaire.Runtime.Azure.CosmosDB.Core
{
    internal class HsCosmosSession : IDisposable
    {
        #region Construct
        readonly CosmosClient cosmosClient;
        readonly Database cosmosDatabase;
        readonly Container cosmosContainer;
        public HsCosmosSession(CosmosClient cosmosClient, Database cosmosDatabase, Container cosmosContainer)
        {
            this.cosmosClient = cosmosClient;
            this.cosmosDatabase = cosmosDatabase;
            this.cosmosContainer = cosmosContainer;
        }
        #endregion

        public CosmosClient Client => cosmosClient;
        public Database Database => cosmosDatabase;
        public Container Container => cosmosContainer;

        public void Dispose()
        {
            new Action(() =>
            {
                if (cosmosClient != null)
                    cosmosClient.Dispose();
            })
            .TryOrFailWithGrace();
        }
    }
}
