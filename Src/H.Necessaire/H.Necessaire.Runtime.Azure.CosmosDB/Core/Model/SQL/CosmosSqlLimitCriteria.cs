namespace H.Necessaire.Runtime.Azure.CosmosDB.Core.Model.SQL
{
    public class CosmosSqlLimitCriteria
    {
        public int Offset { get; set; } = 0;
        public int Count { get; set; } = 10;
    }
}
