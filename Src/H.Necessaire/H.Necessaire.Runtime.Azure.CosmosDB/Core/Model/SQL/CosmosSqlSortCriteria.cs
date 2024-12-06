namespace H.Necessaire.Runtime.Azure.CosmosDB.Core.Model.SQL
{
    public class CosmosSqlSortCriteria
    {
        public string Property { get; set; }
        public string SortDirection { get; set; } = "ASC";
    }
}
