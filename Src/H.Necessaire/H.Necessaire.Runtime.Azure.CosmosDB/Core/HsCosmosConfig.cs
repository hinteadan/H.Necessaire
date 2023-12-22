namespace H.Necessaire.Runtime.Azure.CosmosDB.Core
{
    public class HsCosmosConfig
    {
        public string ApplicationName { get; set; } = "H.Necessaire.Runtime.Azure.CosmosDB";
        public string AccountEndpoint { get; set; }
        public string PrimaryAccessKey { get; set; }
        public string[] ExtraAccessKeys { get; set; }
        public string DatabaseID { get; set; }
        public string ContainerID { get; set; }
    }
}
